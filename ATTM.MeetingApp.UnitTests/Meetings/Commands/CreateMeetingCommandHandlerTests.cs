using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;
using ATTM.MeetingApp.Domain.Entities;
using ATTM.MeetingApp.UnitTests.Mocks;
using Moq;

namespace ATTM.MeetingApp.UnitTests.Meetings.Commands;

public class CreateMeetingCommandHandlerTests
{
    private readonly Mock<IMeetingRepository> _repository = RepositoryMocks.GetMeetingRepository();

    [Fact]
    public async Task CreateMeetingCommandHandler_ValidCommand_AddsMeeting()
    {
        var handler = new CreateMeetingCommandHandler(_repository.Object, MapperFactory.Create());

        var id = await handler.Handle(new CreateMeetingCommand("Meeting 3", LocationsEnum.Lobby, 30), CancellationToken.None);

        var meeting = await _repository.Object.GetAsync(id);
        Assert.NotNull(meeting);
        Assert.Equal("Meeting 3", meeting.Name);
        Assert.Equal(LocationsEnum.Lobby, meeting.Location);
        Assert.Equal(30, meeting.Seats);
    }
}
