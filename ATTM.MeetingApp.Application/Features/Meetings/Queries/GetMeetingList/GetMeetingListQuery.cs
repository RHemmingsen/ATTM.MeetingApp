using System;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList
{
    public record GetMeetingListQuery : IRequest<List<MeetingListVM>>
    {
        
    }
}
