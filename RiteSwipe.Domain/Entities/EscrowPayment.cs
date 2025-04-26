using System.ComponentModel.DataAnnotations;
using RiteSwipe.Domain.Common;

namespace RiteSwipe.Domain.Entities;

public class EscrowPayment : BaseEntity
{
    [Key]
    public int EscrowId { get; set; }
    public int TaskId { get; set; }
    public decimal AmountHeld { get; set; }
    public string PaymentStatus { get; set; } = "Held"; // Held, Released, Disputed
    public DateTime? ReleasedAt { get; set; }

    // Navigation properties
    public TaskItem Task { get; set; } = null!;
}
