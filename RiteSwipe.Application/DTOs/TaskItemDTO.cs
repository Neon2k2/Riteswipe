using System;

namespace RiteSwipe.Application.DTOs;

public class TaskItemDTO
{
    public int TaskId { get; set; }
    public int PostedByUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal CurrentRate { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public string Status { get; set; } = string.Empty;
    public int SkillRequiredId { get; set; }
    public string SkillName { get; set; } = string.Empty;
    public string PostedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
