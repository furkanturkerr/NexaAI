using NexaAI.Domain.Entities;

namespace NexaAI.Application.Interfaces.Services;

public interface IAIService
{
    Task<string> GetResponseAsync(
        List<Message> messages,
        CancellationToken cancellationToken = default);
}