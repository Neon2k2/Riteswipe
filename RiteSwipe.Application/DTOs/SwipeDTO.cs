using System;

namespace RiteSwipe.Application.DTOs;

public class SwipeDTO
{
    public int SwipeId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TaskId { get; set; }
    public string Direction { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
