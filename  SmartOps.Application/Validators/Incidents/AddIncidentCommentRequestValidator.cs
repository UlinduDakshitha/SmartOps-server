using FluentValidation;
using SmartOps.Application.DTOs.Incidents;

namespace SmartOps.Application.Validators.Incidents;

public class AddIncidentCommentRequestValidator
    : AbstractValidator<AddIncidentCommentRequest>
{
    public AddIncidentCommentRequestValidator()
    {
        RuleFor(x => x.Comment)
            .NotEmpty()
            .WithMessage("Comment is required.")
            .MaximumLength(2000)
            .WithMessage("Comment cannot exceed 2000 characters.");
    }
}