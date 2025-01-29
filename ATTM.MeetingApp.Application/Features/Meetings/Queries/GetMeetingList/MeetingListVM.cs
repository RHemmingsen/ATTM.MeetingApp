using System;
using ATTM.MeetingApp.Domain.Entities;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList
{
    public class MeetingListVM
    {
        public LocationsEnum Location { get; set; }
        public int Seats { get; set; }
    }
}
