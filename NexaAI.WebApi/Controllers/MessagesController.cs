using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaAI.Application.Features.Message.Commands;
using NexaAI.Application.Features.Message.Queries;

namespace NexaAI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{conversationId}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid conversationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var query = new GetMessageQuery
            {
                ConversationId = conversationId,
                UserId = userId
            };

            var values = await _mediator.Send(query);

            return Ok(values);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateMessageCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            command.UserId = userId;

            await _mediator.Send(command);

            return Ok();
        }
    }
}
