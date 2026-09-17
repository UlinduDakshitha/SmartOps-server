using FluentValidation;
using SmartOps.Application.DTOs.Incidents;

namespace SmartOps.Application.Validators.Incidents;

public class AssignIncidentRequestValidator : AbstractValidator<AssignIncidentRequest>
{
    public AssignIncidentRequestValidator()
    {
        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("Team is required.");

        RuleFor(x => x.UserId)
            .Must(userId => !userId.HasValue || userId.Value != Guid.Empty)
            .WithMessage("User ID must be valid when provided.");
    }
}