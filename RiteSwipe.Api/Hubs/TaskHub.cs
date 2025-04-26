using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Api.Hubs;

[Authorize]
public class TaskHub : Hub
{
    private readonly ILogger<TaskHub> _logger;

    public TaskHub(ILogger<TaskHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            _logger.LogInformation("User {UserId} connected to TaskHub", userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            _logger.LogInformation("User {UserId} disconnected from TaskHub", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinTaskGroup(string taskId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Task_{taskId}");
        _logger.LogInformation("User joined task group {TaskId}", taskId);
    }

    public async Task LeaveTaskGroup(string taskId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Task_{taskId}");
        _logger.LogInformation("User left task group {TaskId}", taskId);
    }
}
