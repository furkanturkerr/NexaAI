using Microsoft.AspNetCore.SignalR;
using NexaAI.Application.Interfaces.Services;
using NexaAI.WebApi.Hubs;

namespace NexaAI.WebApi.Services;

public class SignalRService : IRealtimeService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendAIChunkAsync(string userId, Guid conversationId, string chunk, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId).SendAsync("ReceiveAIChunk", conversationId, chunk, cancellationToken);
    }

    public async Task SendAICompletedAsync(string userId, Guid conversationId, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.User(userId).SendAsync("ReceiveAICompleted", conversationId, cancellationToken);
    }
}