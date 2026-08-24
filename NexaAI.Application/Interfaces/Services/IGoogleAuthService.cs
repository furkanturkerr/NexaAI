using NexaAI.Application.Features.Auth.Results;

namespace NexaAI.Application.Interfaces.Services;

public interface IGoogleAuthService
{
    Task<GoogleUserResult> LoginAsync(string idToken);
}