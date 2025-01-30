using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATTM.MeetingApp.Domain.Entities;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting
{
    public record UpdateMeetingCommand(Guid id, string Name, LocationsEnum Location, int Seats) : IRequest;
}
