namespace CampusFix.Models;

public class AdminDashboardViewModel
{
    public int TotalRequests { get; set; }
    public int OpenRequests { get; set; }
    public int ResolvedRequests { get; set; }
    public int HighPriorityRequests { get; set; }
    public List<ServiceRequest> RecentRequests { get; set; } = new();
    public Dictionary<string, int> RequestsByCategory { get; set; } = new();
}
