using System.ComponentModel.DataAnnotations;

namespace CampusFix.Models;

public class ServiceRequest
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = "Other";

    [Required]
    [StringLength(20)]
    public string Priority { get; set; } = "Medium";

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Submitted";

    [StringLength(100)]
    public string? Location { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Required]
    public string CreatedByUserId { get; set; } = string.Empty;

    public string? AssignedToUserId { get; set; }

    [StringLength(500)]
    public string? AttachmentUrl { get; set; }

    public ICollection<RequestComment> Comments { get; set; } = new List<RequestComment>();

    public ICollection<RequestStatusHistory> StatusHistory { get; set; } = new List<RequestStatusHistory>();
}
