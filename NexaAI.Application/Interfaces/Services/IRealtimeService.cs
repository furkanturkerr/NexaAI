namespace NexaAI.Application.Interfaces.Services;

public interface IRealtimeService
{
    Task SendAIChunkAsync(string userId, Guid conversationId, string chunk, CancellationToken cancellationToken = default);

    Task SendAICompletedAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default);
}