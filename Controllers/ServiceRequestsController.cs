using System.Security.Claims;
using CampusFix.Data;
using CampusFix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusFix.Controllers;

[Authorize]
public class ServiceRequestsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServiceRequestsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> MyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var requests = await _context.ServiceRequests
            .Where(r => r.CreatedByUserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(requests);
    }

    public IActionResult Create()
    {
        return View(new ServiceRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceRequest model)
    {
        ModelState.Remove(nameof(ServiceRequest.CreatedByUserId));
        ModelState.Remove(nameof(ServiceRequest.Comments));
        ModelState.Remove(nameof(ServiceRequest.StatusHistory));

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        model.CreatedAt = DateTime.UtcNow;
        model.Status = "Submitted";

        _context.ServiceRequests.Add(model);
        await _context.SaveChangesAsync();

        _context.RequestStatusHistory.Add(new RequestStatusHistory
        {
            ServiceRequestId = model.Id,
            OldStatus = null,
            NewStatus = "Submitted",
            ChangedByUserId = model.CreatedByUserId,
            ChangedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(MyRequests));
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isStaff = User.IsInRole("Admin") || User.IsInRole("Technician");

        var request = await _context.ServiceRequests
            .Include(r => r.Comments)
            .Include(r => r.StatusHistory)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            return NotFound();
        }

        if (!isStaff && request.CreatedByUserId != userId)
        {
            return Forbid();
        }

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int serviceRequestId, string commentText)
    {
        if (string.IsNullOrWhiteSpace(commentText))
        {
            return RedirectToAction(nameof(Details), new { id = serviceRequestId });
        }

        var request = await _context.ServiceRequests.FindAsync(serviceRequestId);
        if (request == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isStaff = User.IsInRole("Admin") || User.IsInRole("Technician");

        if (!isStaff && request.CreatedByUserId != userId)
        {
            return Forbid();
        }

        _context.RequestComments.Add(new RequestComment
        {
            ServiceRequestId = serviceRequestId,
            UserId = userId,
            CommentText = commentText.Trim(),
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = serviceRequestId });
    }
}
