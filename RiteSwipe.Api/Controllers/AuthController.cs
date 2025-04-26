using Microsoft.AspNetCore.Mvc;
using RiteSwipe.Application.Authentication.Commands.RegisterUser;
using RiteSwipe.Application.Authentication.Queries.Login;

namespace RiteSwipe.Api.Controllers;

public class AuthController : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<int>> Register(RegisterUserCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginQuery query)
    {
        return await Mediator.Send(query);
    }
}
