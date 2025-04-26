using RiteSwipe.Domain.Common;

namespace RiteSwipe.Domain.Entities;

public class Swipe : BaseEntity
{
    public int SwipeId { get; set; }
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public string SwipeDirection { get; set; } = string.Empty; // "left" or "right"

    // Navigation properties
    public User User { get; set; } = null!;
    public TaskItem Task { get; set; } = null!;
}
