using MediatR;

namespace RiteSwipe.Application.Authentication.Queries.Login;

public record LoginQuery : IRequest<LoginResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
