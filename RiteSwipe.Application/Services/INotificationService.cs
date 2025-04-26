using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Application.Services;

public interface INotificationService
{
    Task<NotificationDTO> CreateNotificationAsync(int userId, string message);
    Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(int userId, bool includeRead = false);
    Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
    Task<bool> DeleteNotificationAsync(int notificationId, int userId);
    Task<int> GetUnreadNotificationCountAsync(int userId);
    Task NotifyTaskStatusChangeAsync(int taskId, string status);
    Task NotifyNewApplicationAsync(int taskId, int applicantId);
    Task NotifyApplicationStatusChangeAsync(int applicationId, string status);
    Task NotifyNewDisputeAsync(int taskId, int disputeId);
    Task NotifyDisputeResolutionAsync(int disputeId);
}
