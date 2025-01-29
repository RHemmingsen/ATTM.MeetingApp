using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.UnitTests.Mocks
{
    public class RepositoryMocks
    {
        public static Mock<IAsyncRepository<Meeting>> GetMeetingRepository()
        {
            var meetings = new List<Meeting>
            {
                new Meeting { Id = Guid.NewGuid(), Name = "Meeting 1", Location = LocationsEnum.Auditorium, Seats = 10 },
                new Meeting { Id = Guid.NewGuid(), Name = "Meeting 2", Location = LocationsEnum.ConferenceRoom, Seats = 20 }
            };
            var mockMeetingRepository = new Mock<IAsyncRepository<Meeting>>();

            mockMeetingRepository.Setup(repo => repo.ListAllAsync()).ReturnsAsync(meetings);
            mockMeetingRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => meetings.FirstOrDefault(x => x.Id == id));
            
            mockMeetingRepository.Setup(repo => repo.AddAsync(It.IsAny<Meeting>()))
                .ReturnsAsync((Meeting meeting) =>
                {
                    meetings.Add(meeting);
                    return meeting;
                });
            return mockMeetingRepository;
        }
    }
}
