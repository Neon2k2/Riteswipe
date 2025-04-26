using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Application.Services;

public interface IEscrowService
{
    Task<EscrowPaymentDTO> CreateEscrowPaymentAsync(int taskId, decimal amount);
    Task<EscrowPaymentDTO?> GetEscrowPaymentByTaskIdAsync(int taskId);
    Task<bool> ReleasePaymentAsync(int taskId, int userId);
    Task<bool> RefundPaymentAsync(int taskId, int userId);
    Task<decimal> GetHeldAmountForUserAsync(int userId);
    Task<IEnumerable<EscrowPaymentDTO>> GetUserEscrowPaymentsAsync(int userId);
    Task<bool> ValidatePaymentAmountAsync(int taskId, decimal amount);
    Task<bool> IsPaymentHeldAsync(int taskId);
}
