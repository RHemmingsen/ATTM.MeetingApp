using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Exceptions;
using ATTM.MeetingApp.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting
{
    public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, Guid>
    {
        readonly IAsyncRepository<Meeting> _meetingRepository;
        readonly IMapper _mapper;

        public CreateMeetingCommandHandler(IMapper mapper, IAsyncRepository<Meeting> meetingRepository)
        {
            _mapper = mapper;
            _meetingRepository = meetingRepository; 
        }

        public async Task<Guid> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateMeetingCommandValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Count > 0)
                throw new ValidationException(validationResult);

            var meeting = _mapper.Map<Meeting>(request);
            meeting = await _meetingRepository.AddAsync(meeting);

            return meeting.Id;
        }
    }
}
