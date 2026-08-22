using MediatR;
using NexaAI.Application.Features.Auth.Results;

namespace NexaAI.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<LoginResult>
{
    public string UserNameOrEmail { get; set; }
    public string Password { get; set; }
}