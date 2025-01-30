using ATTM.MeetingApp.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting
{
public record CreateMeetingCommand(Guid Id, string Name, LocationsEnum Location, int Seats) : IRequest<Guid>;
}
