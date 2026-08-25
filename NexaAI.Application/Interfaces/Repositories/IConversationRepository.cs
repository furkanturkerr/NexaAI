using NexaAI.Domain.Entities;

namespace NexaAI.Application.Interfaces.Repositories;

public interface IConversationRepository
{
    Task CreateAsync(Conversation conversation);
    Task<List<Conversation>> GetConversationsAsync(string userId);
    Task<Conversation?> GetByIdAsync(Guid id);
    Task UpdateAsync(Conversation conversation);
}