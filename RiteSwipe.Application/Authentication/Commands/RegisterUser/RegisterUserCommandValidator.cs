using FluentValidation;

namespace RiteSwipe.Application.Authentication.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(v => v.Password)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(100);

        RuleFor(v => v.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(v => v.PhoneNumber)
            .MaximumLength(20);

        RuleFor(v => v.Bio)
            .MaximumLength(500);
    }
}
