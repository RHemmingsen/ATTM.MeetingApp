using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Exceptions;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;
using ATTM.MeetingApp.UnitTests.Mocks;
using Moq;

namespace ATTM.MeetingApp.UnitTests.Meetings.Queries;

public class GetMeetingQueryHandlerTests
{
    private readonly Mock<IMeetingRepository> _repository = RepositoryMocks.GetMeetingRepository();

    [Fact]
    public async Task GetMeetingQueryHandler_ExistingMeeting_ReturnsVM()
    {
        var handler = new GetMeetingQueryHandler(_repository.Object, MapperFactory.Create());

        var result = await handler.Handle(new GetMeetingQuery(RepositoryMocks.Meeting1Id), CancellationToken.None);

        Assert.Equal(RepositoryMocks.Meeting1Id, result.Id);
        Assert.Equal("Meeting 1", result.Name);
    }

    [Fact]
    public async Task GetMeetingQueryHandler_UnknownId_ThrowsNotFound()
    {
        var handler = new GetMeetingQueryHandler(_repository.Object, MapperFactory.Create());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetMeetingQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
