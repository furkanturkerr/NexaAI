using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaAI.Application.Features.Conversation.Commands;
using NexaAI.Application.Features.Conversation.Queries;

namespace NexaAI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConversationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var values = await _mediator.Send(new GetConversationsQuery(userId));
            return Ok(values);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateConversationCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }
            
            command.UserId = userId!;
            
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteConversationCommand(id));
            return Ok();
        }
    }
}
