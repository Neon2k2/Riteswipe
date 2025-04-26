using Microsoft.EntityFrameworkCore;
using RiteSwipe.Application.Common.Exceptions;
using RiteSwipe.Application.DTOs;
using RiteSwipe.Application.Services;
using RiteSwipe.Domain.Entities;
using RiteSwipe.Infrastructure.Persistence;

namespace RiteSwipe.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public TaskService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<TaskItemDTO> CreateTaskAsync(TaskItemDTO taskDto, int userId)
    {
        var skill = await _context.Skills.FindAsync(taskDto.SkillRequiredId)
            ?? throw new NotFoundException(nameof(Skill), taskDto.SkillRequiredId);

        var task = new TaskItem
        {
            PostedByUserId = userId,
            Title = taskDto.Title,
            Description = taskDto.Description,
            MinPrice = taskDto.MinPrice,
            MaxPrice = taskDto.MaxPrice,
            CurrentRate = taskDto.MinPrice,
            Location = taskDto.Location,
            Deadline = taskDto.Deadline,
            Status = "Open",
            SkillRequiredId = taskDto.SkillRequiredId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return await GetTaskByIdAsync(task.TaskId) 
            ?? throw new ApplicationException("Failed to create task");
    }

    public async Task<TaskItemDTO?> GetTaskByIdAsync(int taskId)
    {
        var task = await _context.Tasks
            .Include(t => t.PostedByUser)
            .Include(t => t.RequiredSkill)
            .FirstOrDefaultAsync(t => t.TaskId == taskId);

        return task == null ? null : MapToDTO(task);
    }

    public async Task<IEnumerable<TaskItemDTO>> GetTasksForSwipingAsync(int userId, int pageSize = 10)
    {
        var userSkills = await _context.UserSkills
            .Where(us => us.UserId == userId)
            .Select(us => us.SkillId)
            .ToListAsync();

        var swipedTaskIds = await _context.Swipes
            .Where(s => s.UserId == userId)
            .Select(s => s.TaskId)
            .ToListAsync();

        var appliedTaskIds = await _context.TaskApplications
            .Where(ta => ta.WorkerId == userId)
            .Select(ta => ta.TaskId)
            .ToListAsync();

        var tasks = await _context.Tasks
            .Include(t => t.PostedByUser)
            .Include(t => t.RequiredSkill)
            .Where(t => t.Status == "Open" &&
                       t.PostedByUserId != userId &&
                       !swipedTaskIds.Contains(t.TaskId) &&
                       !appliedTaskIds.Contains(t.TaskId) &&
                       userSkills.Contains(t.SkillRequiredId))
            .OrderByDescending(t => t.CreatedAt)
            .Take(pageSize)
            .ToListAsync();

        return tasks.Select(MapToDTO);
    }

    public async Task<IEnumerable<TaskItemDTO>> GetUserPostedTasksAsync(int userId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.PostedByUser)
            .Include(t => t.RequiredSkill)
            .Where(t => t.PostedByUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(MapToDTO);
    }

    public async Task<IEnumerable<TaskItemDTO>> GetUserAppliedTasksAsync(int userId)
    {
        var taskIds = await _context.TaskApplications
            .Where(ta => ta.WorkerId == userId)
            .Select(ta => ta.TaskId)
            .ToListAsync();

        var tasks = await _context.Tasks
            .Include(t => t.PostedByUser)
            .Include(t => t.RequiredSkill)
            .Where(t => taskIds.Contains(t.TaskId))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(MapToDTO);
    }

    public async Task<TaskItemDTO> UpdateTaskAsync(int taskId, TaskItemDTO taskDto, int userId)
    {
        var task = await _context.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.PostedByUserId != userId)
            throw new ForbiddenException("You don't have permission to update this task");

        if (task.Status != "Open")
            throw new ValidationException("Cannot update a task that is not open");

        task.Title = taskDto.Title;
        task.Description = taskDto.Description;
        task.MinPrice = taskDto.MinPrice;
        task.MaxPrice = taskDto.MaxPrice;
        task.Location = taskDto.Location;
        task.Deadline = taskDto.Deadline;
        task.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDTO(task);
    }

    public async Task<bool> DeleteTaskAsync(int taskId, int userId)
    {
        var task = await _context.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.PostedByUserId != userId)
            throw new ForbiddenException("You don't have permission to delete this task");

        if (task.Status != "Open")
            throw new ValidationException("Cannot delete a task that is not open");

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SwipeTaskAsync(int taskId, int userId, string direction)
    {
        var task = await _context.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.PostedByUserId == userId)
            throw new ValidationException("Cannot swipe on your own task");

        if (task.Status != "Open")
            throw new ValidationException("Cannot swipe on a closed task");

        var existingSwipe = await _context.Swipes
            .FirstOrDefaultAsync(s => s.TaskId == taskId && s.UserId == userId);

        if (existingSwipe != null)
            throw new ConflictException("Already swiped on this task");

        var swipe = new Swipe
        {
            TaskId = taskId,
            UserId = userId,
            Direction = direction,
            CreatedAt = DateTime.UtcNow
        };

        _context.Swipes.Add(swipe);
        await _context.SaveChangesAsync();

        if (direction == "right")
        {
            await _notificationService.NotifyTaskStatusChangeAsync(taskId, "Swiped Right");
        }

        return true;
    }

    public async Task<TaskApplicationDTO> ApplyToTaskAsync(int taskId, TaskApplicationDTO applicationDto, int userId)
    {
        var task = await _context.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.PostedByUserId == userId)
            throw new ValidationException("Cannot apply to your own task");

        if (task.Status != "Open")
            throw new ValidationException("Cannot apply to a closed task");

        var existingApplication = await _context.TaskApplications
            .FirstOrDefaultAsync(ta => ta.TaskId == taskId && ta.WorkerId == userId);

        if (existingApplication != null)
            throw new ConflictException("Already applied to this task");

        var application = new TaskApplication
        {
            TaskId = taskId,
            WorkerId = userId,
            CoverLetter = applicationDto.CoverLetter,
            ProposedRate = applicationDto.ProposedRate,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskApplications.Add(application);
        await _context.SaveChangesAsync();

        await _notificationService.NotifyNewApplicationAsync(taskId, userId);

        return MapToApplicationDTO(application);
    }

    public async Task<bool> UpdateTaskStatusAsync(int taskId, string status, int userId)
    {
        var task = await _context.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(nameof(TaskItem), taskId);

        if (task.PostedByUserId != userId)
            throw new ForbiddenException("You don't have permission to update this task's status");

        task.Status = status;
        task.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await _notificationService.NotifyTaskStatusChangeAsync(taskId, status);

        return true;
    }

    public async Task<TaskReviewDTO> ReviewTaskAsync(TaskReviewDTO reviewDto, int userId)
    {
        var task = await _context.Tasks.FindAsync(reviewDto.TaskId)
            ?? throw new NotFoundException(nameof(TaskItem), reviewDto.TaskId);

        if (task.Status != "Completed")
            throw new ValidationException("Can only review completed tasks");

        var review = new TaskReview
        {
            TaskId = reviewDto.TaskId,
            ReviewerUserId = userId,
            ReviewedUserId = reviewDto.ReviewedUserId,
            Rating = reviewDto.Rating,
            Comment = reviewDto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskReviews.Add(review);
        await _context.SaveChangesAsync();

        return MapToReviewDTO(review);
    }

    public async Task<TaskDisputeDTO> CreateDisputeAsync(TaskDisputeDTO disputeDto, int userId)
    {
        var task = await _context.Tasks.FindAsync(disputeDto.TaskId)
            ?? throw new NotFoundException(nameof(TaskItem), disputeDto.TaskId);

        var dispute = new TaskDispute
        {
            TaskId = disputeDto.TaskId,
            RaisedByUserId = userId,
            Reason = disputeDto.Reason,
            Description = disputeDto.Description,
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };

        _context.TaskDisputes.Add(dispute);
        await _context.SaveChangesAsync();

        await _notificationService.NotifyNewDisputeAsync(disputeDto.TaskId, dispute.DisputeId);

        return MapToDisputeDTO(dispute);
    }

    private static TaskItemDTO MapToDTO(TaskItem task)
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

    private static TaskApplicationDTO MapToApplicationDTO(TaskApplication application)
    {
        return new TaskApplicationDTO
        {
            ApplicationId = application.ApplicationId,
            TaskId = application.TaskId,
            WorkerId = application.WorkerId,
            CoverLetter = application.CoverLetter,
            ProposedRate = application.ProposedRate,
            Status = application.Status,
            CreatedAt = application.CreatedAt,
            ModifiedAt = application.ModifiedAt
        };
    }

    private static TaskReviewDTO MapToReviewDTO(TaskReview review)
    {
        return new TaskReviewDTO
        {
            ReviewId = review.ReviewId,
            TaskId = review.TaskId,
            ReviewerUserId = review.ReviewerUserId,
            ReviewedUserId = review.ReviewedUserId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            ModifiedAt = review.ModifiedAt
        };
    }

    private static TaskDisputeDTO MapToDisputeDTO(TaskDispute dispute)
    {
        return new TaskDisputeDTO
        {
            DisputeId = dispute.DisputeId,
            TaskId = dispute.TaskId,
            RaisedByUserId = dispute.RaisedByUserId,
            Reason = dispute.Reason,
            Description = dispute.Description,
            Status = dispute.Status,
            Resolution = dispute.Resolution,
            ResolvedAt = dispute.ResolvedAt,
            CreatedAt = dispute.CreatedAt,
            ModifiedAt = dispute.ModifiedAt
        };
    }
}
