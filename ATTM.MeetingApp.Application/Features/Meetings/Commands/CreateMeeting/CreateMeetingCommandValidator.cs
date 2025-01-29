using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting
{
    public class CreateMeetingCommandValidator : AbstractValidator<CreateMeetingCommand>
    {
        public CreateMeetingCommandValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MaximumLength(100).WithMessage("{PropertyName} must not exceed 100 characters.");
            RuleFor(p => p.Location)
                .NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(p => p.Seats)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0.");
        }
    }
}
