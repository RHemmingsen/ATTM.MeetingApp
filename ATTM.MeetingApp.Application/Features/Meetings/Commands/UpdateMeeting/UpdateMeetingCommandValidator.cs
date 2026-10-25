using ATTM.MeetingApp.Domain.Entities;
using FluentValidation;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting;

public class UpdateMeetingCommandValidator : AbstractValidator<UpdateMeetingCommand>
{
    public UpdateMeetingCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(Meeting.NameMaxLength).WithMessage("{PropertyName} must not exceed {MaxLength} characters.");

        RuleFor(p => p.Location)
            .IsInEnum().WithMessage("{PropertyName} is not a valid location.");

        RuleFor(p => p.Seats)
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
    }
}
