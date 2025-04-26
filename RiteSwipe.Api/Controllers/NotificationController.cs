using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiteSwipe.Application.DTOs;
using RiteSwipe.Application.Services;

namespace RiteSwipe.Api.Controllers;

[Authorize]
public class NotificationController : BaseApiController
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotifications([FromQuery] bool includeRead = false)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, includeRead);
        return Ok(notifications);
    }

    [HttpGet("unread/count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var count = await _notificationService.GetUnreadNotificationCountAsync(userId);
        return Ok(count);
    }

    [HttpPost("{id}/read")]
    public async Task<ActionResult> MarkAsRead(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        await _notificationService.MarkNotificationAsReadAsync(id, userId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteNotification(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        await _notificationService.DeleteNotificationAsync(id, userId);
        return NoContent();
    }
}
