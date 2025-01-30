using ATTM.MeetingApp.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting
{
    public class CreateMeetingCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public LocationsEnum Location { get; set; }
        public int Seats { get; set; }
    }
}
