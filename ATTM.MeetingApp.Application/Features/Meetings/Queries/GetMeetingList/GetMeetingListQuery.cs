using System;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList
{
    public class GetMeetingListQuery : IRequest<List<MeetingListVM>>
    {
        
    }
}
