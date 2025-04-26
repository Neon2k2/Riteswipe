using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Application.Services;

public interface ITaskService
{
    Task<TaskItemDTO> CreateTaskAsync(TaskItemDTO taskDto, int userId);
    Task<TaskItemDTO?> GetTaskByIdAsync(int taskId);
    Task<IEnumerable<TaskItemDTO>> GetTasksForSwipingAsync(int userId, int pageSize = 10);
    Task<IEnumerable<TaskItemDTO>> GetUserPostedTasksAsync(int userId);
    Task<IEnumerable<TaskItemDTO>> GetUserAppliedTasksAsync(int userId);
    Task<TaskItemDTO> UpdateTaskAsync(int taskId, TaskItemDTO taskDto, int userId);
    Task<bool> DeleteTaskAsync(int taskId, int userId);
    Task<bool> SwipeTaskAsync(int taskId, int userId, string direction);
    Task<TaskApplicationDTO> ApplyToTaskAsync(int taskId, TaskApplicationDTO applicationDto, int userId);
    Task<bool> UpdateTaskStatusAsync(int taskId, string status, int userId);
    Task<TaskReviewDTO> ReviewTaskAsync(TaskReviewDTO reviewDto, int userId);
    Task<TaskDisputeDTO> CreateDisputeAsync(TaskDisputeDTO disputeDto, int userId);
}
