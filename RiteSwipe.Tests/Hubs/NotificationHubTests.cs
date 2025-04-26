using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using RiteSwipe.Api.Hubs;
using System.Security.Claims;
using Xunit;

namespace RiteSwipe.Tests.Hubs;

public class NotificationHubTests
{
    private readonly Mock<ILogger<NotificationHub>> _loggerMock;
    private readonly Mock<HubCallerContext> _contextMock;
    private readonly Mock<IGroupManager> _groupsMock;
    private readonly NotificationHub _hub;

    public NotificationHubTests()
    {
        _loggerMock = new Mock<ILogger<NotificationHub>>();
        _contextMock = new Mock<HubCallerContext>();
        _groupsMock = new Mock<IGroupManager>();

        _hub = new NotificationHub(_loggerMock.Object)
        {
            Context = _contextMock.Object,
            Groups = _groupsMock.Object
        };
    }

    [Fact]
    public async Task OnConnectedAsync_WithValidUser_AddsToUserGroup()
    {
        // Arrange
        var userId = "test-user-id";
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var claimsIdentity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _contextMock.Setup(c => c.User).Returns(claimsPrincipal);
        _contextMock.Setup(c => c.ConnectionId).Returns("test-connection-id");

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _groupsMock.Verify(
            g => g.AddToGroupAsync(
                "test-connection-id",
                $"User_{userId}",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task OnDisconnectedAsync_WithValidUser_RemovesFromUserGroup()
    {
        // Arrange
        var userId = "test-user-id";
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
        var claimsIdentity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _contextMock.Setup(c => c.User).Returns(claimsPrincipal);
        _contextMock.Setup(c => c.ConnectionId).Returns("test-connection-id");

        // Act
        await _hub.OnDisconnectedAsync(null);

        // Assert
        _groupsMock.Verify(
            g => g.RemoveFromGroupAsync(
                "test-connection-id",
                $"User_{userId}",
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }
}
