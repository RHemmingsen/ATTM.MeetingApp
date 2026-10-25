using ATTM.MeetingApp.Domain.Entities;
using FluentValidation;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;

public class CreateMeetingCommandValidator : AbstractValidator<CreateMeetingCommand>
{
    public CreateMeetingCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(Meeting.NameMaxLength).WithMessage("{PropertyName} must not exceed {MaxLength} characters.");

        RuleFor(p => p.Location)
            .IsInEnum().WithMessage("{PropertyName} is not a valid location.");

        RuleFor(p => p.Seats)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
    }
}
