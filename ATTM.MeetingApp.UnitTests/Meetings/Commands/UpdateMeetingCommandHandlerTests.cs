using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Exceptions;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting;
using ATTM.MeetingApp.Domain.Entities;
using ATTM.MeetingApp.UnitTests.Mocks;
using Moq;

namespace ATTM.MeetingApp.UnitTests.Meetings.Commands;

public class UpdateMeetingCommandHandlerTests
{
    private readonly Mock<IMeetingRepository> _repository = RepositoryMocks.GetMeetingRepository();

    [Fact]
    public async Task UpdateMeetingCommandHandler_ExistingMeeting_UpdatesFields()
    {
        var handler = new UpdateMeetingCommandHandler(_repository.Object, MapperFactory.Create());

        await handler.Handle(new UpdateMeetingCommand(RepositoryMocks.Meeting1Id, "Renamed", LocationsEnum.Office, 5), CancellationToken.None);

        var meeting = await _repository.Object.GetAsync(RepositoryMocks.Meeting1Id);
        Assert.NotNull(meeting);
        Assert.Equal("Renamed", meeting.Name);
        Assert.Equal(LocationsEnum.Office, meeting.Location);
        Assert.Equal(5, meeting.Seats);
        _repository.Verify(r => r.UpdateAsync(meeting, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMeetingCommandHandler_UnknownId_ThrowsNotFound()
    {
        var handler = new UpdateMeetingCommandHandler(_repository.Object, MapperFactory.Create());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new UpdateMeetingCommand(Guid.NewGuid(), "Name", LocationsEnum.Office, 5), CancellationToken.None));
    }
}
