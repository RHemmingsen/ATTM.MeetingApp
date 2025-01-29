using ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList;
using ATTM.MeetingApp.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Application
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Meeting, MeetingVM>().ReverseMap();
            CreateMap<Meeting, MeetingListVM>().ReverseMap();
            CreateMap<Meeting, CreateMeetingCommand>().ReverseMap();
            CreateMap<Meeting, UpdateMeetingCommand>().ReverseMap();
        }
    }
}
