using FluentValidation;
using SmartOps.Application.DTOs.Users;

namespace SmartOps.Application.Validators.Users;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Please enter a valid email address.");
    }
}