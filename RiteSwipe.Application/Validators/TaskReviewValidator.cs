using FluentValidation;
using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Application.Validators;

public class TaskReviewValidator : AbstractValidator<TaskReviewDTO>
{
    public TaskReviewValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.ReviewerId)
            .NotEmpty().WithMessage("Reviewer ID is required");

        RuleFor(x => x.Rating)
            .NotEmpty().WithMessage("Rating is required")
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Review comment is required")
            .MaximumLength(500).WithMessage("Comment must not exceed 500 characters");

        RuleFor(x => x.ReviewType)
            .NotEmpty().WithMessage("Review type is required")
            .Must(type => type == "TaskOwnerReview" || type == "WorkerReview")
            .WithMessage("Review type must be either 'TaskOwnerReview' or 'WorkerReview'");
    }
}
