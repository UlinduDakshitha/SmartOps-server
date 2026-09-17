using FluentValidation;
using SmartOps.Application.DTOs.Incidents;

namespace SmartOps.Application.Validators.Incidents;

public class CreateIncidentRequestValidator : AbstractValidator<CreateIncidentRequest>
{
    public CreateIncidentRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Incident title is required.")
            .MaximumLength(200)
            .WithMessage("Incident title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Incident description is required.")
            .MaximumLength(5000)
            .WithMessage("Incident description cannot exceed 5000 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid priority.");

        RuleFor(x => x.Severity)
            .IsInEnum()
            .WithMessage("Invalid severity.");
    }
}