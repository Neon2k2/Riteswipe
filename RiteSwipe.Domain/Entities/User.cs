using RiteSwipe.Domain.Common;

namespace RiteSwipe.Domain.Entities;

public class User : BaseEntity
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public bool IsVerified { get; set; }

    // Navigation properties
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    public ICollection<TaskItem> PostedTasks { get; set; } = new List<TaskItem>();
    public ICollection<TaskApplication> TaskApplications { get; set; } = new List<TaskApplication>();
    public ICollection<TaskReview> ReviewsGiven { get; set; } = new List<TaskReview>();
    public ICollection<TaskReview> ReviewsReceived { get; set; } = new List<TaskReview>();
    public ICollection<TaskDispute> DisputesRaised { get; set; } = new List<TaskDispute>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Swipe> Swipes { get; set; } = new List<Swipe>();
}
