using FluentValidation;
using SmartOps.Application.DTOs.SLA;

namespace SmartOps.Application.Validators.SLA;

public class UpdateSLAPolicyRequestValidator
    : AbstractValidator<UpdateSLAPolicyRequest>
{
    public UpdateSLAPolicyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("SLA policy name is required.")
            .MaximumLength(100)
            .WithMessage("SLA policy name cannot exceed 100 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid priority.");

        RuleFor(x => x.ResponseTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Response time must be greater than zero.");

        RuleFor(x => x.ResolutionTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Resolution time must be greater than zero.");
    }
}