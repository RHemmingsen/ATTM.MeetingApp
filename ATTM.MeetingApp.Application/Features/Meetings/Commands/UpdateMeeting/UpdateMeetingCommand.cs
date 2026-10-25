using ATTM.MeetingApp.Domain.Entities;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting;

// Id is bound from the route by the controller (`command with { Id = id }`).
public record UpdateMeetingCommand(
    Guid Id,
    string Name,
    LocationsEnum Location,
    int Seats
) : IRequest;
