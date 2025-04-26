namespace RiteSwipe.Domain.Entities;

public class UserSkill
{
    public int UserId { get; set; }
    public int SkillId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
