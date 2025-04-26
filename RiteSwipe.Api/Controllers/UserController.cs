using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiteSwipe.Application.DTOs;
using RiteSwipe.Application.Services;

namespace RiteSwipe.Api.Controllers;

[Authorize]
public class UserController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly ISkillService _skillService;
    private readonly IIdentityService _identityService;
    private readonly JwtService _jwtService;

    public UserController(
        IUserService userService,
        ISkillService skillService,
        IIdentityService identityService,
        JwtService jwtService)
    {
        _userService = userService;
        _skillService = skillService;
        _identityService = identityService;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterRequest request)
    {
        var (success, userId) = await _identityService.CreateUserAsync(request.Email, request.Password);
        if (!success)
        {
            return BadRequest("User with this email already exists");
        }

        var userDto = new UserDTO
        {
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Bio = request.Bio
        };

        var user = await _userService.CreateUserAsync(userDto);
        var token = _jwtService.GenerateJwtToken(user.UserId.ToString(), user.Email);
        return Ok(new { User = user, Token = token });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login([FromBody] LoginRequest request)
    {
        var isValid = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);
        if (!isValid)
        {
            return Unauthorized();
        }

        var user = await _userService.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return NotFound();
        }

        var token = _jwtService.GenerateJwtToken(user.UserId.ToString(), user.Email);
        return Ok(new { User = user, Token = token });
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserDTO>> GetProfile()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<UserDTO>> UpdateProfile([FromBody] UserDTO userDto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        userDto.UserId = userId;
        var user = await _userService.UpdateUserAsync(userDto);
        return Ok(user);
    }

    [HttpPost("skills")]
    public async Task<ActionResult<UserDTO>> AddSkill([FromBody] int skillId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var user = await _userService.AddSkillToUserAsync(userId, skillId);
        return Ok(user);
    }

    [HttpDelete("skills/{skillId}")]
    public async Task<ActionResult<UserDTO>> RemoveSkill(int skillId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var user = await _userService.RemoveSkillFromUserAsync(userId, skillId);
        return Ok(user);
    }

    [HttpGet("{id}/reviews")]
    public async Task<ActionResult<IEnumerable<TaskReviewDTO>>> GetUserReviews(int id)
    {
        var reviews = await _userService.GetUserReviewsAsync(id);
        return Ok(reviews);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<UserStatsDTO>> GetUserStats()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var stats = await _userService.GetUserStatsAsync(userId);
        return Ok(stats);
    }
}

public record RegisterRequest(string Email, string Password, string FullName, string? PhoneNumber, string? Bio);
public record LoginRequest(string Email, string Password);
