using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Application.Services;

public interface ISkillService
{
    Task<SkillDTO> CreateSkillAsync(string skillName);
    Task<SkillDTO?> GetSkillByIdAsync(int skillId);
    Task<SkillDTO?> GetSkillByNameAsync(string skillName);
    Task<IEnumerable<SkillDTO>> GetAllSkillsAsync();
    Task<IEnumerable<SkillDTO>> SearchSkillsAsync(string searchTerm);
    Task<bool> DeleteSkillAsync(int skillId);
    Task<IEnumerable<TaskItemDTO>> GetTasksBySkillAsync(int skillId);
    Task<IEnumerable<UserDTO>> GetUsersBySkillAsync(int skillId);
    Task<int> GetSkillDemandAsync(int skillId);
}
