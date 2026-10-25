using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Exceptions;
using ATTM.MeetingApp.Domain.Entities;
using AutoMapper;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting;

public class UpdateMeetingCommandHandler(IMeetingRepository meetingRepository, IMapper mapper)
    : IRequestHandler<UpdateMeetingCommand>
{
    public async Task Handle(UpdateMeetingCommand request, CancellationToken cancellationToken)
    {
        var meeting = await meetingRepository.GetAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Meeting), request.Id);

        mapper.Map(request, meeting);
        await meetingRepository.UpdateAsync(meeting, cancellationToken);
    }
}
