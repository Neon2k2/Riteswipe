using FluentValidation;
using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Application.Validators;

public class TaskApplicationValidator : AbstractValidator<TaskApplicationDTO>
{
    public TaskApplicationValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.WorkerId)
            .NotEmpty().WithMessage("Worker ID is required");

        RuleFor(x => x.ProposedRate)
            .GreaterThan(0).WithMessage("Proposed rate must be greater than 0");

        RuleFor(x => x.CoverLetter)
            .NotEmpty().WithMessage("Cover letter is required")
            .MaximumLength(1000).WithMessage("Cover letter must not exceed 1000 characters");

        RuleFor(x => x.EstimatedDuration)
            .GreaterThan(0).WithMessage("Estimated duration must be greater than 0");
    }
}
