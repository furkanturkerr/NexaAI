using MediatR;
using NexaAI.Application.Features.Auth.Results;

namespace NexaAI.Application.Features.Auth.Commands;

public class GoogleLoginCommand : IRequest<LoginResult>
{
    public string IdToken { get; set; } = string.Empty;
}