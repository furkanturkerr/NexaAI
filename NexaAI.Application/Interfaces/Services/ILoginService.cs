using NexaAI.Application.Features.Auth.Results;

namespace NexaAI.Application.Interfaces.Services;

public interface ILoginService
{
    Task<LoginUserResult> LoginAsync(string userNameOrEmail, string password);
}