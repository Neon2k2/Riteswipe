using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RiteSwipe.Application.Common.Exceptions;
using RiteSwipe.Application.DTOs;
using RiteSwipe.Application.Services;
using RiteSwipe.Domain.Entities;
using RiteSwipe.Infrastructure.Persistence;

namespace RiteSwipe.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<UserDTO?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Skills)
            .ThenInclude(us => us.Skill)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return user == null ? null : MapToDTO(user);
    }

    public async Task<UserDTO?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .Include(u => u.Skills)
            .ThenInclude(us => us.Skill)
            .FirstOrDefaultAsync(u => u.Email == email);

        return user == null ? null : MapToDTO(user);
    }

    public async Task<UserDTO> CreateUserAsync(UserDTO userDto, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
        {
            throw new ConflictException("Email already registered");
        }

        var user = new User
        {
            FullName = userDto.FullName,
            Email = userDto.Email,
            PasswordHash = HashPassword(password),
            PhoneNumber = userDto.PhoneNumber,
            ProfilePicture = userDto.ProfilePicture,
            Bio = userDto.Bio,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToDTO(user);
    }

    public async Task<UserDTO> UpdateUserAsync(int userId, UserDTO userDto)
    {
        var user = await _context.Users.FindAsync(userId) 
            ?? throw new NotFoundException(nameof(User), userId);

        user.FullName = userDto.FullName;
        user.PhoneNumber = userDto.PhoneNumber;
        user.ProfilePicture = userDto.ProfilePicture;
        user.Bio = userDto.Bio;
        user.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDTO(user);
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddUserSkillAsync(int userId, int skillId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        var skill = await _context.Skills.FindAsync(skillId)
            ?? throw new NotFoundException(nameof(Skill), skillId);

        if (await _context.UserSkills.AnyAsync(us => us.UserId == userId && us.SkillId == skillId))
        {
            return false;
        }

        _context.UserSkills.Add(new UserSkill { UserId = userId, SkillId = skillId });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveUserSkillAsync(int userId, int skillId)
    {
        var userSkill = await _context.UserSkills
            .FirstOrDefaultAsync(us => us.UserId == userId && us.SkillId == skillId);

        if (userSkill == null) return false;

        _context.UserSkills.Remove(userSkill);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SkillDTO>> GetUserSkillsAsync(int userId)
    {
        var skills = await _context.UserSkills
            .Where(us => us.UserId == userId)
            .Include(us => us.Skill)
            .Select(us => new SkillDTO
            {
                SkillId = us.SkillId,
                SkillName = us.Skill.SkillName,
                CreatedAt = us.Skill.CreatedAt,
                ModifiedAt = us.Skill.ModifiedAt
            })
            .ToListAsync();

        return skills;
    }

    public async Task<double> GetUserRatingAsync(int userId)
    {
        var reviews = await _context.TaskReviews
            .Where(r => r.ReviewedUserId == userId)
            .ToListAsync();

        if (!reviews.Any()) return 0;

        return reviews.Average(r => r.Rating);
    }

    public async Task<IEnumerable<TaskReviewDTO>> GetUserReviewsAsync(int userId)
    {
        var reviews = await _context.TaskReviews
            .Where(r => r.ReviewedUserId == userId)
            .Include(r => r.ReviewerUser)
            .Select(r => new TaskReviewDTO
            {
                ReviewId = r.ReviewId,
                TaskId = r.TaskId,
                ReviewerUserId = r.ReviewerUserId,
                ReviewerName = r.ReviewerUser.FullName,
                ReviewerProfilePicture = r.ReviewerUser.ProfilePicture,
                ReviewedUserId = r.ReviewedUserId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                ModifiedAt = r.ModifiedAt
            })
            .ToListAsync();

        return reviews;
    }

    public async Task<bool> VerifyUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId)
            ?? throw new NotFoundException(nameof(User), userId);

        user.IsVerified = true;
        user.ModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GenerateJwtTokenAsync(UserDTO user)
    {
        var secret = _configuration["JwtSettings:Secret"] 
            ?? throw new InvalidOperationException("JWT Secret not configured");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new("IsVerified", user.IsVerified.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> ValidatePasswordAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return false;

        return VerifyPassword(password, user.PasswordHash);
    }

    private static string HashPassword(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        var hashBytes = new byte[salt.Length + hash.Length];
        Array.Copy(salt, 0, hashBytes, 0, salt.Length);
        Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

        return Convert.ToBase64String(hashBytes);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var hashBytes = Convert.FromBase64String(storedHash);
        var salt = new byte[64];
        Array.Copy(hashBytes, 0, salt, 0, salt.Length);

        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != hashBytes[i + salt.Length]) return false;
        }

        return true;
    }

    private static UserDTO MapToDTO(User user)
    {
        return new UserDTO
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = user.ProfilePicture,
            Bio = user.Bio,
            IsVerified = user.IsVerified,
            Skills = user.Skills?.Select(us => us.Skill.SkillName).ToList() ?? new List<string>(),
            CreatedAt = user.CreatedAt,
            ModifiedAt = user.ModifiedAt
        };
    }
}
