using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.DeleteMeeting
{
    public class DeleteMeetingCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
