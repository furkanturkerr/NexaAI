using MediatR;
using NexaAI.Application.Features.Auth.Commands;
using NexaAI.Application.Features.Auth.Results;
using NexaAI.Application.Interfaces.Services;

namespace NexaAI.Application.Features.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly ILoginService _loginService;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(ILoginService loginService, IJwtService jwtService)
    {
        _loginService = loginService;
        _jwtService = jwtService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginResult = await _loginService.LoginAsync(request.UserNameOrEmail, request.Password);

        if (!loginResult.Succeeded)
        {
            return new LoginResult
            {
                Succeeded = false,
                Message = loginResult.Message
            };
        }

        var token = _jwtService.GenerateToken(loginResult.UserId!, loginResult.Email!, loginResult.Name!,
            loginResult.Surname!);
        
        return new LoginResult
        {
            Succeeded = true,
            Token = token,
            Message = "Giriş başarılı."
        };
    }
}