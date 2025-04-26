using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using RiteSwipe.Api.Hubs;
using RiteSwipe.Api.Services;
using RiteSwipe.Application.DTOs;
using Xunit;

namespace RiteSwipe.Tests.Services;

public class SignalRNotificationServiceTests
{
    private readonly Mock<IHubContext<NotificationHub>> _notificationHubMock;
    private readonly Mock<IHubContext<TaskHub>> _taskHubMock;
    private readonly Mock<ILogger<SignalRNotificationService>> _loggerMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly SignalRNotificationService _service;

    public SignalRNotificationServiceTests()
    {
        _notificationHubMock = new Mock<IHubContext<NotificationHub>>();
        _taskHubMock = new Mock<IHubContext<TaskHub>>();
        _loggerMock = new Mock<ILogger<SignalRNotificationService>>();
        _clientProxyMock = new Mock<IClientProxy>();

        _service = new SignalRNotificationService(
            _notificationHubMock.Object,
            _taskHubMock.Object,
            _loggerMock.Object
        );

        var notificationClientsHub = new Mock<IHubClients>();
        notificationClientsHub.Setup(h => h.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
        _notificationHubMock.Setup(h => h.Clients).Returns(notificationClientsHub.Object);

        var taskClientsHub = new Mock<IHubClients>();
        taskClientsHub.Setup(h => h.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
        _taskHubMock.Setup(h => h.Clients).Returns(taskClientsHub.Object);
    }

    [Fact]
    public async Task SendNotificationToUserAsync_SendsNotificationToUserGroup()
    {
        // Arrange
        var userId = "test-user-id";
        var notification = new NotificationDTO
        {
            Id = "test-notification-id",
            UserId = userId,
            Title = "Test Notification",
            Message = "Test Message"
        };

        // Act
        await _service.SendNotificationToUserAsync(userId, notification);

        // Assert
        _clientProxyMock.Verify(
            c => c.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(o => o[0] == notification),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task SendTaskUpdateAsync_SendsUpdateToTaskGroup()
    {
        // Arrange
        var taskId = "test-task-id";
        var task = new TaskItemDTO
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description"
        };

        // Act
        await _service.SendTaskUpdateAsync(taskId, task);

        // Assert
        _clientProxyMock.Verify(
            c => c.SendCoreAsync(
                "TaskUpdated",
                It.Is<object[]>(o => o[0] == task),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task SendEscrowUpdateAsync_SendsUpdateToTaskGroup()
    {
        // Arrange
        var taskId = "test-task-id";
        var escrow = new EscrowPaymentDTO
        {
            Id = "test-escrow-id",
            TaskId = taskId,
            Amount = 100
        };

        // Act
        await _service.SendEscrowUpdateAsync(taskId, escrow);

        // Assert
        _clientProxyMock.Verify(
            c => c.SendCoreAsync(
                "EscrowUpdated",
                It.Is<object[]>(o => o[0] == escrow),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
