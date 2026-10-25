using ATTM.MeetingApp.Domain.Entities;
using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;

public record CreateMeetingCommand(
    string Name,
    LocationsEnum Location,
    int Seats
) : IRequest<Guid>;
