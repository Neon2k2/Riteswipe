using Microsoft.EntityFrameworkCore;
using RiteSwipe.Application.Common.Exceptions;
using RiteSwipe.Application.DTOs;
using RiteSwipe.Application.Services;
using RiteSwipe.Domain.Entities;
using RiteSwipe.Infrastructure.Persistence;

namespace RiteSwipe.Infrastructure.Services;

public class SkillService : ISkillService
{
    private readonly ApplicationDbContext _context;

    public SkillService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SkillDTO> CreateSkillAsync(string skillName)
    {
        if (await _context.Skills.AnyAsync(s => s.SkillName == skillName))
        {
            throw new ConflictException($"Skill '{skillName}' already exists");
        }

        var skill = new Skill
        {
            SkillName = skillName,
            CreatedAt = DateTime.UtcNow
        };

        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        return await GetSkillByIdAsync(skill.SkillId) 
            ?? throw new ApplicationException("Failed to create skill");
    }

    public async Task<SkillDTO?> GetSkillByIdAsync(int skillId)
    {
        var skill = await _context.Skills
            .Include(s => s.Tasks)
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.SkillId == skillId);

        return skill == null ? null : MapToDTO(skill);
    }

    public async Task<SkillDTO?> GetSkillByNameAsync(string skillName)
    {
        var skill = await _context.Skills
            .Include(s => s.Tasks)
            .Include(s => s.Users)
            .FirstOrDefaultAsync(s => s.SkillName == skillName);

        return skill == null ? null : MapToDTO(skill);
    }

    public async Task<IEnumerable<SkillDTO>> GetAllSkillsAsync()
    {
        var skills = await _context.Skills
            .Include(s => s.Tasks)
            .Include(s => s.Users)
            .OrderBy(s => s.SkillName)
            .ToListAsync();

        return skills.Select(MapToDTO);
    }

    public async Task<IEnumerable<SkillDTO>> SearchSkillsAsync(string searchTerm)
    {
        var skills = await _context.Skills
            .Include(s => s.Tasks)
            .Include(s => s.Users)
            .Where(s => s.SkillName.Contains(searchTerm))
            .OrderBy(s => s.SkillName)
            .ToListAsync();

        return skills.Select(MapToDTO);
    }

    public async Task<bool> DeleteSkillAsync(int skillId)
    {
        var skill = await _context.Skills.FindAsync(skillId)
            ?? throw new NotFoundException(nameof(Skill), skillId);

        if (await _context.Tasks.AnyAsync(t => t.SkillRequiredId == skillId))
        {
            throw new ValidationException("Cannot delete a skill that is required by tasks");
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TaskItemDTO>> GetTasksBySkillAsync(int skillId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.PostedByUser)
            .Include(t => t.RequiredSkill)
            .Where(t => t.SkillRequiredId == skillId && t.Status == "Open")
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(MapToTaskDTO);
    }

    public async Task<IEnumerable<UserDTO>> GetUsersBySkillAsync(int skillId)
    {
        var users = await _context.UserSkills
            .Include(us => us.User)
            .Where(us => us.SkillId == skillId)
            .Select(us => us.User)
            .ToListAsync();

        return users.Select(MapToUserDTO);
    }

    public async Task<int> GetSkillDemandAsync(int skillId)
    {
        return await _context.Tasks
            .CountAsync(t => t.SkillRequiredId == skillId && t.Status == "Open");
    }

    private static SkillDTO MapToDTO(Skill skill)
    {
        return new SkillDTO
        {
            SkillId = skill.SkillId,
            SkillName = skill.SkillName,
            TaskCount = skill.Tasks?.Count ?? 0,
            UserCount = skill.Users?.Count ?? 0,
            CreatedAt = skill.CreatedAt,
            ModifiedAt = skill.ModifiedAt
        };
    }

    private static TaskItemDTO MapToTaskDTO(TaskItem task)
    {
        return new TaskItemDTO
        {
            TaskId = task.TaskId,
            PostedByUserId = task.PostedByUserId,
            Title = task.Title,
            Description = task.Description,
            MinPrice = task.MinPrice,
            MaxPrice = task.MaxPrice,
            CurrentRate = task.CurrentRate,
            Location = task.Location,
            Deadline = task.Deadline,
            Status = task.Status,
            SkillRequiredId = task.SkillRequiredId,
            SkillName = task.RequiredSkill?.SkillName ?? string.Empty,
            PostedByUserName = task.PostedByUser?.FullName ?? string.Empty,
            CreatedAt = task.CreatedAt,
            ModifiedAt = task.ModifiedAt
        };
    }

    private static UserDTO MapToUserDTO(User user)
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
