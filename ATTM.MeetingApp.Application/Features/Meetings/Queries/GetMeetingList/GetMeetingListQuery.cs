using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList;

public record GetMeetingListQuery : IRequest<List<MeetingListVM>>;
