using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RiteSwipe.Application.Common.Interfaces;
using RiteSwipe.Infrastructure.Persistence;

namespace RiteSwipe.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly ApplicationDbContext _context;

    public IdentityService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string UserId)> CreateUserAsync(string email, string password)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            return (false, string.Empty);
        }

        var passwordHash = HashPassword(password);
        var user = new Domain.Entities.User
        {
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(CancellationToken.None);

        return (true, user.UserId.ToString());
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return false;
        }

        var passwordHash = HashPassword(password);
        return user.PasswordHash == passwordHash;
    }

    public async Task<string> GetUserNameAsync(string userId)
    {
        var user = await _context.Users.FindAsync(int.Parse(userId));
        return user?.FullName ?? string.Empty;
    }

    public Task<bool> IsInRoleAsync(string userId, string role)
    {
        // Implement role-based authorization if needed
        return Task.FromResult(true);
    }

    public Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        // Implement policy-based authorization if needed
        return Task.FromResult(true);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
