using System;
using System.Drawing;
using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Domain.Entities;
using AutoMapper;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList
{
    public class GetMeetingListQueryHandler : IRequestHandler<GetMeetingListQuery, List<MeetingListVM>>
    {
        readonly IAsyncRepository<Meeting> _meetingRepository;
        readonly IMapper _mapper;

        public GetMeetingListQueryHandler(IMapper mapper, IAsyncRepository<Meeting> meetingRepository)
        {
            _meetingRepository = meetingRepository;
            _mapper = mapper;
        }

        public async Task<List<MeetingListVM>> Handle(GetMeetingListQuery request, CancellationToken cancellationToken)
        {
            var allMeetings = (await _meetingRepository.ListAllAsync()).OrderBy(x => x.Seats);
            return _mapper.Map<List<MeetingListVM>>(allMeetings);
        }
    }
}
