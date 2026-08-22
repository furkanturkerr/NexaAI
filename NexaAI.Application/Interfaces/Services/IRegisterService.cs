using NexaAI.Application.Features.Auth.Results;

namespace NexaAI.Application.Interfaces.Services;

public interface IRegisterService
{
    Task<RegisterResult> RegisterAsync(string name, string surname, string email, string password);
}