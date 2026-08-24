using MediatR;
using NexaAI.Application.Features.Auth.Commands;
using NexaAI.Application.Features.Auth.Results;
using NexaAI.Application.Interfaces.Services;

namespace NexaAI.Application.Features.Auth.Handlers;

public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, LoginResult>
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IJwtService _jwtService;

    public GoogleLoginCommandHandler(IGoogleAuthService googleAuthService, IJwtService jwtService)
    {
        _googleAuthService = googleAuthService;
        _jwtService = jwtService;
    }

    public async Task<LoginResult> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var googleResult = await _googleAuthService.LoginAsync(request.IdToken);

        if (!googleResult.Succeeded)
        {
            return new LoginResult
            {
                Succeeded = false,
                Message = googleResult.Message
            };
        }

        var token = _jwtService.GenerateToken(
            googleResult.UserId!,
            googleResult.Email!,
            googleResult.Name!,
            googleResult.Surname!);

        return new LoginResult
        {
            Succeeded = true,
            Message = "Google ile giriş başarılı.",
            Token = token
        };
    }
}