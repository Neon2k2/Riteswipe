using System.ComponentModel.DataAnnotations;
using RiteSwipe.Domain.Common;

namespace RiteSwipe.Domain.Entities;

public class TaskApplication : BaseEntity
{
    [Key]
    public int ApplicationId { get; set; }
    public int TaskId { get; set; }
    public int WorkerId { get; set; }
    public string ApplicationStatus { get; set; } = "Pending"; // Pending, Accepted, Rejected, Cancelled

    // Navigation properties
    public TaskItem Task { get; set; } = null!;
    public User Worker { get; set; } = null!;
}
