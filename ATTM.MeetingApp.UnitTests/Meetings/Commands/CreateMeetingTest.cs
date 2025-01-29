using ATTM.MeetingApp.Application;
using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;
using ATTM.MeetingApp.Domain.Entities;
using ATTM.MeetingApp.UnitTests.Mocks;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATTM.MeetingApp.UnitTests.Meetings.Commands
{
    public class CreateMeetingTest
    {
        readonly IMapper _mapper;
        readonly Mock<IAsyncRepository<Meeting>> _mockMeetingRepository;

        public CreateMeetingTest()
        {
            _mockMeetingRepository = RepositoryMocks.GetMeetingRepository();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfiles>());

            _mapper = configuration.CreateMapper();
        }
        
        [Fact]
        public async Task CreateMeetingCommandHandler_Success()
        {
            //Arrange
            var handler = new CreateMeetingCommandHandler(_mapper, _mockMeetingRepository.Object);
            var command = new CreateMeetingCommand
            {
                Name = "Meeting 3",
                Location = LocationsEnum.Auditorium,
                Seats = 30
            };

            //Act
            var result = await handler.Handle(command, CancellationToken.None);

            //Assert
            var allMeetings = await _mockMeetingRepository.Object.ListAllAsync();
            Assert.Equal(3, allMeetings.Count);
        }

    }
}
