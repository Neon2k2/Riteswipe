using System.ComponentModel.DataAnnotations;
using RiteSwipe.Domain.Common;

namespace RiteSwipe.Domain.Entities;

public class TaskReview : BaseEntity
{
    [Key]
    public int ReviewId { get; set; }
    public int TaskId { get; set; }
    public int ReviewerUserId { get; set; }
    public int ReviewedUserId { get; set; }
    public int Rating { get; set; }
    public string? ReviewText { get; set; }

    // Navigation properties
    public TaskItem Task { get; set; } = null!;
    public User ReviewerUser { get; set; } = null!;
    public User ReviewedUser { get; set; } = null!;
}
