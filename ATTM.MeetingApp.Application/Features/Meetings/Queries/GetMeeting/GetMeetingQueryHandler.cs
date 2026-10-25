using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Exceptions;
using ATTM.MeetingApp.Domain.Entities;
using AutoMapper;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;

public class GetMeetingQueryHandler(IMeetingRepository meetingRepository, IMapper mapper)
    : IRequestHandler<GetMeetingQuery, MeetingVM>
{
    public async Task<MeetingVM> Handle(GetMeetingQuery request, CancellationToken cancellationToken)
    {
        var meeting = await meetingRepository.GetAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Meeting), request.Id);

        return mapper.Map<MeetingVM>(meeting);
    }
}
