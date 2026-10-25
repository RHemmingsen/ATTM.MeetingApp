using MediatR;

namespace ATTM.MeetingApp.Application.Features.Meetings.Commands.DeleteMeeting;

public record DeleteMeetingCommand(Guid Id) : IRequest;
