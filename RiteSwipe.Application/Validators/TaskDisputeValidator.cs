using FluentValidation;
using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Application.Validators;

public class TaskDisputeValidator : AbstractValidator<TaskDisputeDTO>
{
    public TaskDisputeValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.RaisedByUserId)
            .NotEmpty().WithMessage("User ID who raised the dispute is required");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Dispute reason is required")
            .MaximumLength(1000).WithMessage("Reason must not exceed 1000 characters");

        RuleFor(x => x.Evidence)
            .MaximumLength(2000).WithMessage("Evidence must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Evidence));

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Dispute status is required")
            .Must(status => status == "Open" || status == "UnderReview" || status == "Resolved" || status == "Closed")
            .WithMessage("Invalid dispute status");

        RuleFor(x => x.Resolution)
            .MaximumLength(1000).WithMessage("Resolution must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Resolution));
    }
}
