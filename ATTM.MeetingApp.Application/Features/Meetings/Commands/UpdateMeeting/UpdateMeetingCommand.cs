using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATTM.MeetingApp.Domain.Entities;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting
{
    public class UpdateMeetingCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public LocationsEnum Location { get; set; }
        public int Seats { get; set; }
    }
}
