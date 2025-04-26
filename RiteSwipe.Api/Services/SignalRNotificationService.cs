using Microsoft.AspNetCore.SignalR;
using RiteSwipe.Api.Hubs;
using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Api.Services;

public class SignalRNotificationService
{
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IHubContext<TaskHub> _taskHub;
    private readonly ILogger<SignalRNotificationService> _logger;

    public SignalRNotificationService(
        IHubContext<NotificationHub> notificationHub,
        IHubContext<TaskHub> taskHub,
        ILogger<SignalRNotificationService> logger)
    {
        _notificationHub = notificationHub;
        _taskHub = taskHub;
        _logger = logger;
    }

    public async Task SendNotificationToUserAsync(string userId, NotificationDTO notification)
    {
        try
        {
            await _notificationHub.Clients.Group($"User_{userId}")
                .SendAsync("ReceiveNotification", notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
        }
    }

    public async Task SendTaskUpdateAsync(string taskId, TaskItemDTO task)
    {
        try
        {
            await _taskHub.Clients.Group($"Task_{taskId}")
                .SendAsync("TaskUpdated", task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending task update for task {TaskId}", taskId);
        }
    }

    public async Task SendNewApplicationAsync(string taskId, TaskApplicationDTO application)
    {
        try
        {
            await _taskHub.Clients.Group($"Task_{taskId}")
                .SendAsync("NewApplication", application);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending new application notification for task {TaskId}", taskId);
        }
    }

    public async Task SendTaskMatchAsync(string userId, TaskItemDTO task)
    {
        try
        {
            await _taskHub.Clients.Group($"User_{userId}")
                .SendAsync("TaskMatch", task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending task match notification to user {UserId}", userId);
        }
    }

    public async Task SendEscrowUpdateAsync(string taskId, EscrowPaymentDTO escrow)
    {
        try
        {
            await _taskHub.Clients.Group($"Task_{taskId}")
                .SendAsync("EscrowUpdated", escrow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending escrow update for task {TaskId}", taskId);
        }
    }

    public async Task SendDisputeUpdateAsync(string taskId, TaskDisputeDTO dispute)
    {
        try
        {
            await _taskHub.Clients.Group($"Task_{taskId}")
                .SendAsync("DisputeUpdated", dispute);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending dispute update for task {TaskId}", taskId);
        }
    }

    public async Task SendReviewNotificationAsync(string userId, TaskReviewDTO review)
    {
        try
        {
            await _notificationHub.Clients.Group($"User_{userId}")
                .SendAsync("NewReview", review);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending review notification to user {UserId}", userId);
        }
    }

    public async Task SendUserStatusUpdateAsync(string userId, string status)
    {
        try
        {
            await _notificationHub.Clients.Group($"User_{userId}")
                .SendAsync("StatusUpdate", status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending status update to user {UserId}", userId);
        }
    }
}
