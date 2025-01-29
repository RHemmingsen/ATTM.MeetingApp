using ATTM.MeetingApp.Application.Contracts;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting
{
    public class GetMeetingQueryHandler : IRequestHandler<GetMeetingQuery, MeetingVM>
    {
        readonly IMeetingRepository _meetingRepository;
        readonly IMapper _mapper;

        public GetMeetingQueryHandler(IMeetingRepository meetingRepository, IMapper mapper)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
        }

        public async Task<MeetingVM> Handle(GetMeetingQuery request, CancellationToken cancellationToken)
        {
            var meeting = await _meetingRepository.GetAsync(request.Id);
            return _mapper.Map<MeetingVM>(meeting);
        }
    }
}
