using FluentValidation;
using SmartOps.Application.DTOs.Teams;

namespace SmartOps.Application.Validators.Teams;

public class AddTeamMemberRequestValidator : AbstractValidator<AddTeamMemberRequest>
{
    public AddTeamMemberRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User is required.");
    }
}