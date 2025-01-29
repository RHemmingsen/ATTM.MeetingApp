using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Exceptions;
using ATTM.MeetingApp.Domain.Entities;
using AutoMapper;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting
{
    public class UpdateMeetingCommandHandler : IRequestHandler<UpdateMeetingCommand>
    {
        readonly IAsyncRepository<Meeting> _meetingRepository;
        readonly IMapper _mapper;

        public UpdateMeetingCommandHandler(IAsyncRepository<Meeting> meetingRepository, IMapper mapper)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
        }

        public async Task Handle(UpdateMeetingCommand request, CancellationToken cancellationToken)
        {
            var meetingToUpdate = await _meetingRepository.GetAsync(request.Id);
            if (meetingToUpdate == null)
            {
                throw new NotFoundException(nameof(Meeting), request.Id);
            }

            var validator = new UpdateMeetingCommandValidator();
            var validationResult = await validator.ValidateAsync(request);
            if(validationResult.Errors.Count > 0)
            {
                throw new ValidationException(validationResult);
            }   

            _mapper.Map(request, meetingToUpdate, typeof(UpdateMeetingCommand), typeof(Meeting));

            await _meetingRepository.UpdateAsync(meetingToUpdate);
        }   
    }
}
