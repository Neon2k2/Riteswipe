using System;
using System.Collections.Generic;

namespace RiteSwipe.Application.DTOs;

public class UserDTO
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public bool IsVerified { get; set; }
    public List<string> Skills { get; set; } = new();
    public double AverageRating { get; set; }
    public int CompletedTasks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
