using Microsoft.AspNetCore.Mvc.Testing;
using RiteSwipe.Api;
using RiteSwipe.Api.Controllers;
using RiteSwipe.Application.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace RiteSwipe.Tests.Integration;

public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = $"test_{Guid.NewGuid()}@example.com",
            Password = "Test123!",
            FullName = "Test User",
            PhoneNumber = "+1234567890"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenValidCredentials()
    {
        // Arrange
        var email = $"test_{Guid.NewGuid()}@example.com";
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = "Test123!",
            FullName = "Test User"
        };

        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = "Test123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
    }

    [Fact]
    public async Task GetProfile_ReturnsUserProfile_WhenAuthenticated()
    {
        // Arrange
        var email = $"test_{Guid.NewGuid()}@example.com";
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = "Test123!",
            FullName = "Test User"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResult);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.Token);

        // Act
        var response = await _client.GetAsync("/api/v1/users/profile");

        // Assert
        response.EnsureSuccessStatusCode();
        var profile = await response.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(profile);
        Assert.Equal(registerRequest.Email, profile.Email);
        Assert.Equal(registerRequest.FullName, profile.FullName);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsUpdatedProfile_WhenValidRequest()
    {
        // Arrange
        var email = $"test_{Guid.NewGuid()}@example.com";
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = "Test123!",
            FullName = "Test User"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResult);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.Token);

        var updateRequest = new UserDTO
        {
            Email = email,
            FullName = "Updated Test User",
            Bio = "This is my updated bio",
            Skills = new[] { "testing", "integration" }
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/users/profile", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedProfile = await response.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(updatedProfile);
        Assert.Equal(updateRequest.FullName, updatedProfile.FullName);
        Assert.Equal(updateRequest.Bio, updatedProfile.Bio);
        Assert.Equal(updateRequest.Skills, updatedProfile.Skills);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenInvalidCredentials()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_ReturnsUnauthorized_WhenNotAuthenticated()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/users/profile");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
