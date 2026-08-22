using MediatR;
using NexaAI.Application.Features.Auth.Commands;
using NexaAI.Application.Features.Auth.Results;
using NexaAI.Application.Interfaces.Services;

namespace NexaAI.Application.Features.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IRegisterService _registerService;

    public RegisterCommandHandler(IRegisterService registerService)
    {
        _registerService = registerService;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _registerService.RegisterAsync(request.Name, request.Surname, request.Email, request.Password);
    }
}