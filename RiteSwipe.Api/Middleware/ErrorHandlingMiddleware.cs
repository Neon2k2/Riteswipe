using System.Net;
using System.Text.Json;
using RiteSwipe.Application.Common.Exceptions;

namespace RiteSwipe.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { errors = validationException.Errors });
                break;

            case NotFoundException notFoundException:
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new { error = notFoundException.Message });
                break;

            case ConflictException conflictException:
                code = HttpStatusCode.Conflict;
                result = JsonSerializer.Serialize(new { error = conflictException.Message });
                break;

            case ForbiddenException forbiddenException:
                code = HttpStatusCode.Forbidden;
                result = JsonSerializer.Serialize(new { error = forbiddenException.Message });
                break;

            case UnauthorizedException unauthorizedException:
                code = HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new { error = unauthorizedException.Message });
                break;

            default:
                _logger.LogError(exception, "An unhandled exception occurred");
                result = JsonSerializer.Serialize(new { 
                    error = _environment.IsDevelopment() ? exception.ToString() : "An error occurred processing your request." 
                });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        return context.Response.WriteAsync(result);
    }
}
