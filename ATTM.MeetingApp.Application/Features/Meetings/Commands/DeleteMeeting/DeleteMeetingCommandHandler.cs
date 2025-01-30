using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Exceptions;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.DeleteMeeting;
using ATTM.MeetingApp.Domain.Entities;
using AutoMapper;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.DeleteEvent
{
    public class DeleteMeetingCommandHandler : IRequestHandler<DeleteMeetingCommand>
    {
        readonly IAsyncRepository<Meeting> _meetingRepository;

        public DeleteMeetingCommandHandler(IAsyncRepository<Meeting> meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public async Task Handle(DeleteMeetingCommand request, CancellationToken cancellationToken)
        {
            var meetingToDelete = await _meetingRepository.GetAsync(request.Id);

            meetingToDelete = meetingToDelete ?? throw new NotFoundException(nameof(Meeting), request.Id);
            
            //ArgumentException.ThrowIfNullOrEmpty(meetingToDelete.Id.ToString(), nameof(Meeting));
            await _meetingRepository.DeleteAsync(meetingToDelete);
        }
    }
}
