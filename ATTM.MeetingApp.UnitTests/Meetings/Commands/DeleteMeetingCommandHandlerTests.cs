using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Exceptions;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.DeleteMeeting;
using ATTM.MeetingApp.UnitTests.Mocks;
using Moq;

namespace ATTM.MeetingApp.UnitTests.Meetings.Commands;

public class DeleteMeetingCommandHandlerTests
{
    private readonly Mock<IMeetingRepository> _repository = RepositoryMocks.GetMeetingRepository();

    [Fact]
    public async Task DeleteMeetingCommandHandler_ExistingMeeting_RemovesIt()
    {
        var handler = new DeleteMeetingCommandHandler(_repository.Object);

        await handler.Handle(new DeleteMeetingCommand(RepositoryMocks.Meeting1Id), CancellationToken.None);

        Assert.Null(await _repository.Object.GetAsync(RepositoryMocks.Meeting1Id));
    }

    [Fact]
    public async Task DeleteMeetingCommandHandler_UnknownId_ThrowsNotFound()
    {
        var handler = new DeleteMeetingCommandHandler(_repository.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new DeleteMeetingCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
