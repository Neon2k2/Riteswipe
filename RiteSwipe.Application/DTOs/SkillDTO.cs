using System;

namespace RiteSwipe.Application.DTOs;

public class SkillDTO
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
