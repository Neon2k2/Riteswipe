using Microsoft.EntityFrameworkCore;
using RiteSwipe.Application.Common.Exceptions;
using RiteSwipe.Application.DTOs;
using RiteSwipe.Application.Services;
using RiteSwipe.Domain.Entities;
using RiteSwipe.Infrastructure.Persistence;

namespace RiteSwipe.Infrastructure.Services;

public class EscrowService : IEscrowService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public EscrowService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<EscrowPaymentDTO> CreateEscrowPaymentAsync(int taskId, decimal amount)
    {
        var task = await _context.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (await _context.EscrowPayments.AnyAsync(e => e.TaskId == taskId))
        {
            throw new ConflictException("Escrow payment already exists for this task");
        }

        if (!await ValidatePaymentAmountAsync(taskId, amount))
        {
            throw new ValidationException("Invalid payment amount");
        }

        var escrow = new EscrowPayment
        {
            TaskId = taskId,
            AmountHeld = amount,
            IsReleased = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.EscrowPayments.Add(escrow);
        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            task.PostedByUserId,
            $"Escrow payment of ${amount} created for task '{task.Title}'"
        );

        return MapToDTO(escrow);
    }

    public async Task<EscrowPaymentDTO?> GetEscrowPaymentByTaskIdAsync(int taskId)
    {
        var escrow = await _context.EscrowPayments
            .FirstOrDefaultAsync(e => e.TaskId == taskId);

        return escrow == null ? null : MapToDTO(escrow);
    }

    public async Task<bool> ReleasePaymentAsync(int taskId, int userId)
    {
        var task = await _context.Tasks
            .Include(t => t.EscrowPayment)
            .FirstOrDefaultAsync(t => t.TaskId == taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.PostedByUserId != userId)
        {
            throw new ForbiddenException("Only the task owner can release the payment");
        }

        if (task.EscrowPayment == null)
        {
            throw new ValidationException("No escrow payment found for this task");
        }

        if (task.EscrowPayment.IsReleased)
        {
            throw new ValidationException("Payment has already been released");
        }

        task.EscrowPayment.IsReleased = true;
        task.EscrowPayment.ReleasedAt = DateTime.UtcNow;
        task.EscrowPayment.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Get the assigned worker from the accepted application
        var worker = await _context.TaskApplications
            .Include(ta => ta.Worker)
            .Where(ta => ta.TaskId == taskId && ta.Status == "Accepted")
            .Select(ta => ta.Worker)
            .FirstOrDefaultAsync();

        if (worker != null)
        {
            await _notificationService.CreateNotificationAsync(
                worker.UserId,
                $"Payment of ${task.EscrowPayment.AmountHeld} has been released for task '{task.Title}'"
            );
        }

        return true;
    }

    public async Task<bool> RefundPaymentAsync(int taskId, int userId)
    {
        var task = await _context.Tasks
            .Include(t => t.EscrowPayment)
            .FirstOrDefaultAsync(t => t.TaskId == taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.PostedByUserId != userId)
        {
            throw new ForbiddenException("Only the task owner can request a refund");
        }

        if (task.EscrowPayment == null)
        {
            throw new ValidationException("No escrow payment found for this task");
        }

        if (task.EscrowPayment.IsReleased)
        {
            throw new ValidationException("Cannot refund a released payment");
        }

        // In a real application, you would integrate with a payment provider here
        // to process the refund

        task.EscrowPayment.IsReleased = true;
        task.EscrowPayment.ReleasedAt = DateTime.UtcNow;
        task.EscrowPayment.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _notificationService.CreateNotificationAsync(
            userId,
            $"Refund of ${task.EscrowPayment.AmountHeld} processed for task '{task.Title}'"
        );

        return true;
    }

    public async Task<decimal> GetHeldAmountForUserAsync(int userId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.EscrowPayment)
            .Where(t => t.PostedByUserId == userId && 
                       t.EscrowPayment != null && 
                       !t.EscrowPayment.IsReleased)
            .ToListAsync();

        return tasks.Sum(t => t.EscrowPayment?.AmountHeld ?? 0);
    }

    public async Task<IEnumerable<EscrowPaymentDTO>> GetUserEscrowPaymentsAsync(int userId)
    {
        var escrows = await _context.Tasks
            .Include(t => t.EscrowPayment)
            .Where(t => t.PostedByUserId == userId && t.EscrowPayment != null)
            .Select(t => t.EscrowPayment!)
            .ToListAsync();

        return escrows.Select(MapToDTO);
    }

    public async Task<bool> ValidatePaymentAmountAsync(int taskId, decimal amount)
    {
        var task = await _context.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        // Check if amount is within the task's price range
        return amount >= task.MinPrice && amount <= task.MaxPrice;
    }

    public async Task<bool> IsPaymentHeldAsync(int taskId)
    {
        return await _context.EscrowPayments
            .AnyAsync(e => e.TaskId == taskId && !e.IsReleased);
    }

    private static EscrowPaymentDTO MapToDTO(EscrowPayment escrow)
    {
        return new EscrowPaymentDTO
        {
            EscrowId = escrow.EscrowId,
            TaskId = escrow.TaskId,
            AmountHeld = escrow.AmountHeld,
            IsReleased = escrow.IsReleased,
            ReleasedAt = escrow.ReleasedAt,
            CreatedAt = escrow.CreatedAt,
            ModifiedAt = escrow.ModifiedAt
        };
    }
}
