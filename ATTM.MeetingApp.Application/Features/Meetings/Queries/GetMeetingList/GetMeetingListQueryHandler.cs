using ATTM.MeetingApp.Application.Contracts;
using AutoMapper;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList;

public class GetMeetingListQueryHandler(IMeetingRepository meetingRepository, IMapper mapper)
    : IRequestHandler<GetMeetingListQuery, List<MeetingListVM>>
{
    public async Task<List<MeetingListVM>> Handle(GetMeetingListQuery request, CancellationToken cancellationToken)
    {
        var meetings = await meetingRepository.ListOrderedBySeatsAsync(cancellationToken);
        return mapper.Map<List<MeetingListVM>>(meetings);
    }
}
