using System.ComponentModel.DataAnnotations;

namespace CampusFix.Models;

public class RequestComment
{
    public int Id { get; set; }

    public int ServiceRequestId { get; set; }

    public ServiceRequest? ServiceRequest { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string CommentText { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
