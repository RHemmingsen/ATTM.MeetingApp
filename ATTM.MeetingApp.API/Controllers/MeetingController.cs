using ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.DeleteMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ATTM.MeetingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeetingController(IMediator mediator) : ControllerBase
{
    [HttpGet(Name = "GetMeetingList")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MeetingListVM>>> GetAll(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetMeetingListQuery(), cancellationToken));

    [HttpGet("{id:guid}", Name = "GetMeeting")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MeetingVM>> Get(Guid id, CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetMeetingQuery(id), cancellationToken));

    [HttpPost(Name = "CreateMeeting")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateMeetingCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return CreatedAtRoute("GetMeeting", new { id }, id);
    }

    [HttpPut("{id:guid}", Name = "UpdateMeeting")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateMeetingCommand command, CancellationToken cancellationToken)
    {
        await mediator.Send(command with { Id = id }, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}", Name = "DeleteMeeting")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteMeetingCommand(id), cancellationToken);
        return NoContent();
    }
}
