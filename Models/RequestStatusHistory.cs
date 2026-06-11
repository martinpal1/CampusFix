using System.ComponentModel.DataAnnotations;

namespace CampusFix.Models;

public class RequestStatusHistory
{
    public int Id { get; set; }

    public int ServiceRequestId { get; set; }

    public ServiceRequest? ServiceRequest { get; set; }

    [StringLength(30)]
    public string? OldStatus { get; set; }

    [Required]
    [StringLength(30)]
    public string NewStatus { get; set; } = string.Empty;

    [Required]
    public string ChangedByUserId { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
