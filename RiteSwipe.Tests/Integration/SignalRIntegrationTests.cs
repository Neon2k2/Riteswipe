using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using RiteSwipe.Api;
using RiteSwipe.Application.DTOs;
using System.Net.Http.Headers;
using Xunit;

namespace RiteSwipe.Tests.Integration;

public class SignalRIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SignalRIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var loginRequest = new
        {
            Email = "test@example.com",
            Password = "Test123!"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return result?.Token ?? throw new InvalidOperationException("Failed to get auth token");
    }

    private HubConnection CreateHubConnection(string hubUrl, string token)
    {
        return new HubConnectionBuilder()
            .WithUrl($"{_factory.Server.BaseAddress}{hubUrl}", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .WithAutomaticReconnect()
            .Build();
    }

    [Fact]
    public async Task NotificationHub_ReceivesNotifications_WhenSent()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        var connection = CreateHubConnection("hubs/notification", token);
        await connection.StartAsync();

        var notificationReceived = new TaskCompletionSource<NotificationDTO>();
        connection.On<NotificationDTO>("ReceiveNotification", notification =>
        {
            notificationReceived.SetResult(notification);
        });

        // Create a test notification
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var notificationRequest = new NotificationDTO
        {
            Title = "Test Notification",
            Message = "This is a test notification"
        };

        // Act
        await _client.PostAsJsonAsync("/api/v1/notifications", notificationRequest);

        // Assert
        var receivedNotification = await notificationReceived.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.NotNull(receivedNotification);
        Assert.Equal(notificationRequest.Title, receivedNotification.Title);
        Assert.Equal(notificationRequest.Message, receivedNotification.Message);

        await connection.DisposeAsync();
    }

    [Fact]
    public async Task TaskHub_ReceivesUpdates_WhenTaskModified()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        var connection = CreateHubConnection("hubs/task", token);
        await connection.StartAsync();

        var taskUpdated = new TaskCompletionSource<TaskItemDTO>();
        connection.On<TaskItemDTO>("TaskUpdated", task =>
        {
            taskUpdated.SetResult(task);
        });

        // Create a test task
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var taskRequest = new TaskItemDTO
        {
            Title = "Test Task",
            Description = "This is a test task",
            Budget = 100
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", taskRequest);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemDTO>();
        Assert.NotNull(createdTask);

        // Join the task group
        await connection.InvokeAsync("JoinTaskGroup", createdTask.Id);

        // Act - Update the task
        var updateRequest = new TaskItemDTO
        {
            Id = createdTask.Id,
            Title = "Updated Test Task",
            Description = createdTask.Description,
            Budget = createdTask.Budget
        };

        await _client.PutAsJsonAsync($"/api/v1/tasks/{createdTask.Id}", updateRequest);

        // Assert
        var updatedTask = await taskUpdated.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.NotNull(updatedTask);
        Assert.Equal(updateRequest.Title, updatedTask.Title);
        Assert.Equal(createdTask.Id, updatedTask.Id);

        await connection.DisposeAsync();
    }

    [Fact]
    public async Task TaskHub_ReceivesEscrowUpdates_WhenPaymentMade()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        var connection = CreateHubConnection("hubs/task", token);
        await connection.StartAsync();

        var escrowUpdated = new TaskCompletionSource<EscrowPaymentDTO>();
        connection.On<EscrowPaymentDTO>("EscrowUpdated", escrow =>
        {
            escrowUpdated.SetResult(escrow);
        });

        // Create a test task and escrow payment
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var taskRequest = new TaskItemDTO
        {
            Title = "Test Task for Escrow",
            Description = "This is a test task",
            Budget = 100
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", taskRequest);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemDTO>();
        Assert.NotNull(createdTask);

        // Join the task group
        await connection.InvokeAsync("JoinTaskGroup", createdTask.Id);

        // Act - Create escrow payment
        var escrowRequest = new EscrowPaymentDTO
        {
            TaskId = createdTask.Id,
            Amount = 100
        };

        await _client.PostAsJsonAsync($"/api/v1/tasks/{createdTask.Id}/escrow", escrowRequest);

        // Assert
        var updatedEscrow = await escrowUpdated.Task.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.NotNull(updatedEscrow);
        Assert.Equal(createdTask.Id, updatedEscrow.TaskId);
        Assert.Equal(escrowRequest.Amount, updatedEscrow.Amount);

        await connection.DisposeAsync();
    }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
}
