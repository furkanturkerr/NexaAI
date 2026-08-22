using MediatR;
using NexaAI.Application.Features.Auth.Results;

namespace NexaAI.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<RegisterResult>
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}