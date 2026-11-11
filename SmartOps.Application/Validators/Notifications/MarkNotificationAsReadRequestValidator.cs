using FluentValidation;
using SmartOps.Application.DTOs.Notifications;

namespace SmartOps.Application.Validators.Notifications;

public class MarkNotificationAsReadRequestValidator
    : AbstractValidator<MarkNotificationAsReadRequest>
{
    public MarkNotificationAsReadRequestValidator()
    {
        RuleFor(x => x.NotificationId)
            .NotEmpty()
            .WithMessage("Notification is required.");
    }
}