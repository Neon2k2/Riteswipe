using System.ComponentModel.DataAnnotations;
using RiteSwipe.Domain.Common;

namespace RiteSwipe.Domain.Entities;

public class TaskItem : BaseEntity
{
    [Key]
    public int TaskId { get; set; }
    public int PostedByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? SkillRequiredId { get; set; }
    public string? Location { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal CurrentRate { get; set; }
    public int DemandIndex { get; set; }
    public string Status { get; set; } = "Open";
    public DateTime? Deadline { get; set; }

    // Navigation properties
    public User PostedByUser { get; set; } = null!;
    public Skill? RequiredSkill { get; set; }
    public ICollection<TaskApplication> Applications { get; set; } = new List<TaskApplication>();
    public ICollection<Swipe> Swipes { get; set; } = new List<Swipe>();
    public ICollection<TaskReview> Reviews { get; set; } = new List<TaskReview>();
    public ICollection<TaskDispute> Disputes { get; set; } = new List<TaskDispute>();
    public EscrowPayment? EscrowPayment { get; set; }
}
