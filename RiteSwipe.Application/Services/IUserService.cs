using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Application.Services;

public interface IUserService
{
    Task<UserDTO?> GetUserByIdAsync(int userId);
    Task<UserDTO?> GetUserByEmailAsync(string email);
    Task<UserDTO> CreateUserAsync(UserDTO userDto, string password);
    Task<UserDTO> UpdateUserAsync(int userId, UserDTO userDto);
    Task<bool> DeleteUserAsync(int userId);
    Task<bool> AddUserSkillAsync(int userId, int skillId);
    Task<bool> RemoveUserSkillAsync(int userId, int skillId);
    Task<IEnumerable<SkillDTO>> GetUserSkillsAsync(int userId);
    Task<double> GetUserRatingAsync(int userId);
    Task<IEnumerable<TaskReviewDTO>> GetUserReviewsAsync(int userId);
    Task<bool> VerifyUserAsync(int userId);
    Task<string> GenerateJwtTokenAsync(UserDTO user);
    Task<bool> ValidatePasswordAsync(string email, string password);
}
