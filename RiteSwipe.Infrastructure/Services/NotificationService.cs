using Microsoft.EntityFrameworkCore;
using RiteSwipe.Application.Common.Exceptions;
using RiteSwipe.Application.DTOs;
using RiteSwipe.Application.Services;
using RiteSwipe.Domain.Entities;
using RiteSwipe.Infrastructure.Persistence;

namespace RiteSwipe.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationDTO> CreateNotificationAsync(int userId, string message)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return MapToDTO(notification);
    }

    public async Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(int userId, bool includeRead = false)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId);

        if (!includeRead)
        {
            query = query.Where(n => !n.IsRead);
        }

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(MapToDTO);
    }

    public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId)
            ?? throw new NotFoundException(nameof(Notification), notificationId);

        notification.IsRead = true;
        notification.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId)
            ?? throw new NotFoundException(nameof(Notification), notificationId);

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUnreadNotificationCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task NotifyTaskStatusChangeAsync(int taskId, string status)
    {
        var task = await _context.Tasks
            .Include(t => t.PostedByUser)
            .Include(t => t.Applications)
            .ThenInclude(a => a.Worker)
            .FirstOrDefaultAsync(t => t.TaskId == taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        var message = $"Task '{task.Title}' status changed to {status}";

        // Notify task owner
        await CreateNotificationAsync(task.PostedByUserId, message);

        // Notify applicants
        foreach (var application in task.Applications)
        {
            await CreateNotificationAsync(application.WorkerId, message);
        }
    }

    public async Task NotifyNewApplicationAsync(int taskId, int applicantId)
    {
        var task = await _context.Tasks
            .Include(t => t.PostedByUser)
            .FirstOrDefaultAsync(t => t.TaskId == taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        var applicant = await _context.Users.FindAsync(applicantId)
            ?? throw new NotFoundException(nameof(User), applicantId);

        var message = $"New application received from {applicant.FullName} for task '{task.Title}'";
        await CreateNotificationAsync(task.PostedByUserId, message);
    }

    public async Task NotifyApplicationStatusChangeAsync(int applicationId, string status)
    {
        var application = await _context.TaskApplications
            .Include(ta => ta.Task)
            .Include(ta => ta.Worker)
            .FirstOrDefaultAsync(ta => ta.ApplicationId == applicationId)
            ?? throw new NotFoundException(nameof(TaskApplication), applicationId);

        var message = $"Your application for task '{application.Task.Title}' has been {status}";
        await CreateNotificationAsync(application.WorkerId, message);
    }

    public async Task NotifyNewDisputeAsync(int taskId, int disputeId)
    {
        var dispute = await _context.TaskDisputes
            .Include(td => td.Task)
            .Include(td => td.RaisedByUser)
            .FirstOrDefaultAsync(td => td.DisputeId == disputeId)
            ?? throw new NotFoundException(nameof(TaskDispute), disputeId);

        var message = $"New dispute raised by {dispute.RaisedByUser.FullName} for task '{dispute.Task.Title}'";
        await CreateNotificationAsync(dispute.Task.PostedByUserId, message);
    }

    public async Task NotifyDisputeResolutionAsync(int disputeId)
    {
        var dispute = await _context.TaskDisputes
            .Include(td => td.Task)
            .Include(td => td.RaisedByUser)
            .FirstOrDefaultAsync(td => td.DisputeId == disputeId)
            ?? throw new NotFoundException(nameof(TaskDispute), disputeId);

        var message = $"Dispute for task '{dispute.Task.Title}' has been resolved";
        
        // Notify both the task owner and the dispute raiser
        await CreateNotificationAsync(dispute.Task.PostedByUserId, message);
        await CreateNotificationAsync(dispute.RaisedByUserId, message);
    }

    private static NotificationDTO MapToDTO(Notification notification)
    {
        return new NotificationDTO
        {
            NotificationId = notification.NotificationId,
            UserId = notification.UserId,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ModifiedAt = notification.ModifiedAt
        };
    }
}
