using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Exceptions;
using ATTM.MeetingApp.Domain.Entities;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.DeleteMeeting;

public class DeleteMeetingCommandHandler(IMeetingRepository meetingRepository)
    : IRequestHandler<DeleteMeetingCommand>
{
    public async Task Handle(DeleteMeetingCommand request, CancellationToken cancellationToken)
    {
        var meeting = await meetingRepository.GetAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Meeting), request.Id);

        await meetingRepository.DeleteAsync(meeting, cancellationToken);
    }
}
