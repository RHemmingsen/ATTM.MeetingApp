using ATTM.MeetingApp.Domain.Entities;

namespace ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;

public record MeetingVM
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public LocationsEnum Location { get; init; }
    public int Seats { get; init; }
}
