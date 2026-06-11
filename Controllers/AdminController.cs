using System.Security.Claims;
using CampusFix.Data;
using CampusFix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Controllers;

[Authorize(Roles = "Admin,Technician")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var requests = await _context.ServiceRequests.ToListAsync();

        var model = new AdminDashboardViewModel
        {
            TotalRequests = requests.Count,
            OpenRequests = requests.Count(r => r.Status is not "Resolved" and not "Closed" and not "Rejected"),
            ResolvedRequests = requests.Count(r => r.Status is "Resolved" or "Closed"),
            HighPriorityRequests = requests.Count(r => r.Priority == "High" || r.Priority == "Critical"),
            RecentRequests = requests.OrderByDescending(r => r.CreatedAt).Take(5).ToList(),
            RequestsByCategory = requests.GroupBy(r => r.Category).ToDictionary(g => g.Key, g => g.Count())
        };

        return View(model);
    }

    public async Task<IActionResult> Manage(string? status, string? category)
    {
        var query = _context.ServiceRequests.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(r => r.Category == category);
        }

        var requests = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string newStatus)
    {
        var request = await _context.ServiceRequests.FindAsync(id);
        if (request == null)
        {
            return NotFound();
        }

        var oldStatus = request.Status;
        request.Status = newStatus;
        request.UpdatedAt = DateTime.UtcNow;

        _context.RequestStatusHistory.Add(new RequestStatusHistory
        {
            ServiceRequestId = request.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            ChangedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            ChangedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Assign(int id, string assignedToUserId)
    {
        var request = await _context.ServiceRequests.FindAsync(id);
        if (request == null)
        {
            return NotFound();
        }

        request.AssignedToUserId = assignedToUserId;
        request.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Manage));
    }
}
