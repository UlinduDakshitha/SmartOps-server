using FluentValidation;
using SmartOps.Application.DTOs.Incidents;

namespace SmartOps.Application.Validators.Incidents;

public class ResolveIncidentRequestValidator : AbstractValidator<ResolveIncidentRequest>
{
    public ResolveIncidentRequestValidator()
    {
        RuleFor(x => x.ResolutionNotes)
            .NotEmpty()
            .WithMessage("Resolution notes are required.")
            .MaximumLength(5000)
            .WithMessage("Resolution notes cannot exceed 5000 characters.");

        RuleFor(x => x.RootCause)
            .NotEmpty()
            .WithMessage("Root cause is required.")
            .MaximumLength(2000)
            .WithMessage("Root cause cannot exceed 2000 characters.");
    }
}