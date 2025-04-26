using System;

namespace RiteSwipe.Application.DTOs;

public class TaskReviewDTO
{
    public int ReviewId { get; set; }
    public int TaskId { get; set; }
    public int ReviewerUserId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string? ReviewerProfilePicture { get; set; }
    public int ReviewedUserId { get; set; }
    public string ReviewedUserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
