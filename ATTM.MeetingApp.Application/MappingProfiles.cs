using ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList;
using ATTM.MeetingApp.Domain.Entities;
using AutoMapper;

namespace ATTM.MeetingApp.Application;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // Meeting
        CreateMap<Meeting, MeetingVM>();
        CreateMap<Meeting, MeetingListVM>();
        CreateMap<CreateMeetingCommand, Meeting>();
        CreateMap<UpdateMeetingCommand, Meeting>();
    }
}
