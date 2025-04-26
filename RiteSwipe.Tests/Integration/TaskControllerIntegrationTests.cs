using Microsoft.AspNetCore.Mvc.Testing;
using RiteSwipe.Api;
using RiteSwipe.Application.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace RiteSwipe.Tests.Integration;

public class TaskControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TaskControllerIntegrationTests(WebApplicationFactory<Program> factory)
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

    [Fact]
    public async Task CreateTask_ReturnsCreatedTask_WhenValidRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var taskRequest = new TaskItemDTO
        {
            Title = "Integration Test Task",
            Description = "This is a test task created during integration testing",
            Budget = 100,
            Skills = new[] { "coding", "testing" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/tasks", taskRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdTask = await response.Content.ReadFromJsonAsync<TaskItemDTO>();
        Assert.NotNull(createdTask);
        Assert.Equal(taskRequest.Title, createdTask.Title);
        Assert.Equal(taskRequest.Description, createdTask.Description);
        Assert.Equal(taskRequest.Budget, createdTask.Budget);
    }

    [Fact]
    public async Task GetTask_ReturnsTask_WhenExists()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var taskRequest = new TaskItemDTO
        {
            Title = "Test Task for Get",
            Description = "This is a test task",
            Budget = 100
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", taskRequest);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemDTO>();
        Assert.NotNull(createdTask);

        // Act
        var response = await _client.GetAsync($"/api/v1/tasks/{createdTask.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var retrievedTask = await response.Content.ReadFromJsonAsync<TaskItemDTO>();
        Assert.NotNull(retrievedTask);
        Assert.Equal(createdTask.Id, retrievedTask.Id);
        Assert.Equal(createdTask.Title, retrievedTask.Title);
    }

    [Fact]
    public async Task UpdateTask_ReturnsUpdatedTask_WhenValidRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var taskRequest = new TaskItemDTO
        {
            Title = "Original Task",
            Description = "This is the original task",
            Budget = 100
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", taskRequest);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemDTO>();
        Assert.NotNull(createdTask);

        var updateRequest = new TaskItemDTO
        {
            Id = createdTask.Id,
            Title = "Updated Task",
            Description = "This is the updated task",
            Budget = 150
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/tasks/{createdTask.Id}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedTask = await response.Content.ReadFromJsonAsync<TaskItemDTO>();
        Assert.NotNull(updatedTask);
        Assert.Equal(updateRequest.Title, updatedTask.Title);
        Assert.Equal(updateRequest.Description, updatedTask.Description);
        Assert.Equal(updateRequest.Budget, updatedTask.Budget);
    }

    [Fact]
    public async Task ApplyForTask_ReturnsApplication_WhenValidRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var taskRequest = new TaskItemDTO
        {
            Title = "Task for Application",
            Description = "This is a test task",
            Budget = 100
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", taskRequest);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemDTO>();
        Assert.NotNull(createdTask);

        var applicationRequest = new TaskApplicationDTO
        {
            TaskId = createdTask.Id,
            ProposedPrice = 90,
            Message = "I would like to work on this task"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/tasks/{createdTask.Id}/applications", applicationRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var application = await response.Content.ReadFromJsonAsync<TaskApplicationDTO>();
        Assert.NotNull(application);
        Assert.Equal(createdTask.Id, application.TaskId);
        Assert.Equal(applicationRequest.ProposedPrice, application.ProposedPrice);
    }

    [Fact]
    public async Task CreateEscrowPayment_ReturnsPayment_WhenValidRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var taskRequest = new TaskItemDTO
        {
            Title = "Task for Escrow",
            Description = "This is a test task",
            Budget = 100
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/tasks", taskRequest);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemDTO>();
        Assert.NotNull(createdTask);

        var escrowRequest = new EscrowPaymentDTO
        {
            TaskId = createdTask.Id,
            Amount = 100
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/tasks/{createdTask.Id}/escrow", escrowRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var payment = await response.Content.ReadFromJsonAsync<EscrowPaymentDTO>();
        Assert.NotNull(payment);
        Assert.Equal(createdTask.Id, payment.TaskId);
        Assert.Equal(escrowRequest.Amount, payment.Amount);
    }
}
