namespace RiteSwipe.Application.Authentication.Queries.Login;

public record LoginResponse
{
    public int UserId { get; init; }
    public string Token { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
