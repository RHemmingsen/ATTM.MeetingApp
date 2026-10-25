using ATTM.MeetingApp.Application.Contracts;
using ATTM.MeetingApp.Domain.Entities;
using Moq;

namespace ATTM.MeetingApp.UnitTests.Mocks;

public static class RepositoryMocks
{
    public static readonly Guid Meeting1Id = new("00000000-0000-0000-0000-000000000001");
    public static readonly Guid Meeting2Id = new("00000000-0000-0000-0000-000000000002");

    public static Mock<IMeetingRepository> GetMeetingRepository()
    {
        var meetings = new List<Meeting>
        {
            new() { Id = Meeting1Id, Name = "Meeting 1", Location = LocationsEnum.Auditorium, Seats = 20 },
            new() { Id = Meeting2Id, Name = "Meeting 2", Location = LocationsEnum.ConferenceRoom, Seats = 10 },
        };

        var mock = new Mock<IMeetingRepository>();
        mock.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(meetings);
        mock.Setup(r => r.ListOrderedBySeatsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => meetings.OrderBy(x => x.Seats).ToList());
        mock.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => meetings.FirstOrDefault(x => x.Id == id));
        mock.Setup(r => r.AddAsync(It.IsAny<Meeting>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Meeting meeting, CancellationToken _) =>
            {
                meeting.Id = Guid.NewGuid();
                meetings.Add(meeting);
                return meeting;
            });
        mock.Setup(r => r.UpdateAsync(It.IsAny<Meeting>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mock.Setup(r => r.DeleteAsync(It.IsAny<Meeting>(), It.IsAny<CancellationToken>()))
            .Callback((Meeting meeting, CancellationToken _) => meetings.Remove(meeting))
            .Returns(Task.CompletedTask);
        return mock;
    }
}
