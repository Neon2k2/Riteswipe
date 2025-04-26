using RiteSwipe.Domain.Common;

namespace RiteSwipe.Domain.Entities;

public class Skill : BaseEntity
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
