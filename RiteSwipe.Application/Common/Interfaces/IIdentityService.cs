namespace RiteSwipe.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string UserId)> CreateUserAsync(string email, string password);
    Task<bool> ValidateCredentialsAsync(string email, string password);
    Task<string> GetUserNameAsync(string userId);
    Task<bool> IsInRoleAsync(string userId, string role);
    Task<bool> AuthorizeAsync(string userId, string policyName);
}
