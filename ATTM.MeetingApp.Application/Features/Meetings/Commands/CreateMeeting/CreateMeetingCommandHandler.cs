using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Domain.Entities;
using AutoMapper;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;

public class CreateMeetingCommandHandler(IMeetingRepository meetingRepository, IMapper mapper)
    : IRequestHandler<CreateMeetingCommand, Guid>
{
    public async Task<Guid> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        var meeting = mapper.Map<Meeting>(request);
        meeting = await meetingRepository.AddAsync(meeting, cancellationToken);
        return meeting.Id;
    }
}
