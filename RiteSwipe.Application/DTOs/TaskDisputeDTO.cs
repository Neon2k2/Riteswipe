using System;

namespace RiteSwipe.Application.DTOs;

public class TaskDisputeDTO
{
    public int DisputeId { get; set; }
    public int TaskId { get; set; }
    public int RaisedByUserId { get; set; }
    public string RaisedByUserName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Resolution { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
