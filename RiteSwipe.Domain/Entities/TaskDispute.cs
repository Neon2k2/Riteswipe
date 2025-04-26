using System.ComponentModel.DataAnnotations;
using RiteSwipe.Domain.Common;

namespace RiteSwipe.Domain.Entities;

public class TaskDispute : BaseEntity
{
    [Key]
    public int DisputeId { get; set; }
    public int TaskId { get; set; }
    public int RaisedByUserId { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "Open"; // Open, Resolved, Rejected

    // Navigation properties
    public TaskItem Task { get; set; } = null!;
    public User RaisedByUser { get; set; } = null!;
}
