using FluentValidation;
using SmartOps.Application.DTOs.Teams;

namespace SmartOps.Application.Validators.Teams;

public class CreateTeamRequestValidator : AbstractValidator<CreateTeamRequest>
{
    public CreateTeamRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Team name is required.")
            .MaximumLength(100)
            .WithMessage("Team name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Team description is required.")
            .MaximumLength(500)
            .WithMessage("Team description cannot exceed 500 characters.");
    }
}