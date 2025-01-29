using ATTM.MeetingApp.Application.Features.Meetings.Commands.CreateMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.DeleteMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Commands.UpdateMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeeting;
using ATTM.MeetingApp.Application.Features.Meetings.Queries.GetMeetingList;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATTM.MeetingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingController : Controller
    {
        readonly IMediator _mediator;

        public MeetingController(IMediator mediator)
        {
            _mediator = mediator;  
        }

        [HttpGet(Name = "GetAllMeetings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<MeetingVM>>> GetAllMeetings()
        {
           var results = await _mediator.Send(new GetMeetingListQuery());
           return Ok(results);
        }

        [HttpGet("{id}", Name = "GetMeeting")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MeetingVM>> GetMeeting(Guid id)
        {
            var query = new GetMeetingQuery() { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost(Name = "AddMeeting")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateMeetingCommand createMeetingCommand)
        {
           var id = await _mediator.Send(createMeetingCommand);
           return Ok(id);
        }

        [HttpPut(Name = "UpdateMeeting")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update([FromBody] UpdateMeetingCommand updateMeetingCommand)
        {
           await _mediator.Send(updateMeetingCommand);
           return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteMeeting")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
           var deleteMeetingCommand = new DeleteMeetingCommand() { Id = id };
           await _mediator.Send(deleteMeetingCommand);
           return NoContent();
        }
    }
}
