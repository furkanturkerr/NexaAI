using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexaAI.Application.Features.Auth.Commands;

namespace NexaAI.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }
            
            return Ok(result);
        }
        
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(
            GoogleLoginCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
        
    }
}
