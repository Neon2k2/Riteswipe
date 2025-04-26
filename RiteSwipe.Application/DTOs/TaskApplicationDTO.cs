using System;

namespace RiteSwipe.Application.DTOs;

public class TaskApplicationDTO
{
    public int ApplicationId { get; set; }
    public int TaskId { get; set; }
    public int WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public string? WorkerProfilePicture { get; set; }
    public string CoverLetter { get; set; } = string.Empty;
    public decimal ProposedRate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
