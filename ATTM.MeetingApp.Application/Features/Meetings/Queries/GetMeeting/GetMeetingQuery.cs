using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;

public record GetMeetingQuery(Guid Id) : IRequest<MeetingVM>;
