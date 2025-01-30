using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting
{
    public record GetMeetingQuery(Guid Id) : IRequest<MeetingVM>;
}
