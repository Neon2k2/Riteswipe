using System;

namespace RiteSwipe.Application.DTOs;

public class EscrowPaymentDTO
{
    public int EscrowId { get; set; }
    public int TaskId { get; set; }
    public decimal AmountHeld { get; set; }
    public bool IsReleased { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
