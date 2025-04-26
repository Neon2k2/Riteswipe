using FluentValidation;
using RiteSwipe.Application.DTOs;

namespace RiteSwipe.Application.Validators;

public class TaskItemValidator : AbstractValidator<TaskItemDTO>
{
    public TaskItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum price must be greater than or equal to 0")
            .LessThanOrEqualTo(x => x.MaxPrice).WithMessage("Minimum price must be less than or equal to maximum price");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice).WithMessage("Maximum price must be greater than or equal to minimum price");

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location must not exceed 200 characters");

        RuleFor(x => x.Deadline)
            .GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future")
            .When(x => x.Deadline.HasValue);

        RuleFor(x => x.SkillRequiredId)
            .NotEmpty().WithMessage("Required skill must be specified");
    }
}
