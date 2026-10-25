using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList;
using ATTM.MeetingApp.UnitTests.Mocks;
using Moq;

namespace ATTM.MeetingApp.UnitTests.Meetings.Queries;

public class GetMeetingListQueryHandlerTests
{
    private readonly Mock<IMeetingRepository> _repository = RepositoryMocks.GetMeetingRepository();

    [Fact]
    public async Task GetMeetingListQueryHandler_TwoMeetings_ReturnsThemOrderedBySeats()
    {
        var handler = new GetMeetingListQueryHandler(_repository.Object, MapperFactory.Create());

        var result = await handler.Handle(new GetMeetingListQuery(), CancellationToken.None);

        Assert.Equal([RepositoryMocks.Meeting2Id, RepositoryMocks.Meeting1Id], result.Select(x => x.Id));
    }
}
