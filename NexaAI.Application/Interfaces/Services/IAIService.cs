using NexaAI.Domain.Entities;

namespace NexaAI.Application.Interfaces.Services;

public interface IAIService
{
    IAsyncEnumerable<string> GetResponseStreamAsync(List<Message> messages, CancellationToken cancellationToken = default);
}