using MediatR;

namespace RiteSwipe.Application.Authentication.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<int>
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Bio { get; init; }
}
