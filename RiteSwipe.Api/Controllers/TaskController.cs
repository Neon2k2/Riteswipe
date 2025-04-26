using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiteSwipe.Application.DTOs;
using RiteSwipe.Application.Services;

namespace RiteSwipe.Api.Controllers;

[Authorize]
public class TaskController : BaseApiController
{
    private readonly ITaskService _taskService;
    private readonly INotificationService _notificationService;
    private readonly IEscrowService _escrowService;

    public TaskController(
        ITaskService taskService,
        INotificationService notificationService,
        IEscrowService escrowService)
    {
        _taskService = taskService;
        _notificationService = notificationService;
        _escrowService = escrowService;
    }

    [HttpPost]
    public async Task<ActionResult<TaskItemDTO>> CreateTask([FromBody] TaskItemDTO taskDto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        taskDto.PostedByUserId = userId;
        var task = await _taskService.CreateTaskAsync(taskDto);
        return Ok(task);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItemDTO>> GetTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetTasks([FromQuery] string? status = null)
    {
        var tasks = await _taskService.GetTasksAsync(status);
        return Ok(tasks);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskItemDTO>> UpdateTask(int id, [FromBody] TaskItemDTO taskDto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        taskDto.TaskId = id;
        var task = await _taskService.UpdateTaskAsync(taskDto, userId);
        await _notificationService.NotifyTaskStatusChangeAsync(id, task.Status);
        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        await _taskService.DeleteTaskAsync(id, userId);
        return NoContent();
    }

    [HttpPost("{id}/apply")]
    public async Task<ActionResult<TaskApplicationDTO>> ApplyForTask(int id, [FromBody] TaskApplicationDTO applicationDto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        applicationDto.TaskId = id;
        applicationDto.WorkerId = userId;
        var application = await _taskService.ApplyForTaskAsync(applicationDto);
        await _notificationService.NotifyNewApplicationAsync(id, userId);
        return Ok(application);
    }

    [HttpPost("{id}/swipe")]
    public async Task<ActionResult<SwipeDTO>> SwipeTask(int id, [FromBody] SwipeDTO swipeDto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        swipeDto.TaskId = id;
        swipeDto.UserId = userId;
        var swipe = await _taskService.SwipeTaskAsync(swipeDto);
        return Ok(swipe);
    }

    [HttpPost("{id}/escrow")]
    public async Task<ActionResult<EscrowPaymentDTO>> CreateEscrowPayment(int id, [FromBody] decimal amount)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null || task.PostedByUserId != userId)
        {
            return Forbid();
        }
        var escrow = await _escrowService.CreateEscrowPaymentAsync(id, amount);
        return Ok(escrow);
    }

    [HttpPost("{id}/release")]
    public async Task<ActionResult> ReleaseEscrowPayment(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        await _escrowService.ReleasePaymentAsync(id, userId);
        return NoContent();
    }

    [HttpPost("{id}/refund")]
    public async Task<ActionResult> RefundEscrowPayment(int id)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        await _escrowService.RefundPaymentAsync(id, userId);
        return NoContent();
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetMyTasks([FromQuery] string type = "posted")
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
        var tasks = type.ToLower() switch
        {
            "posted" => await _taskService.GetUserPostedTasksAsync(userId),
            "applied" => await _taskService.GetUserAppliedTasksAsync(userId),
            _ => throw new ArgumentException("Invalid type. Must be 'posted' or 'applied'")
        };
        return Ok(tasks);
    }
}
