using NexaAI.Domain.Entities;

namespace NexaAI.Application.Interfaces.Repositories;

public interface IMessageRepository
{
    Task CreateAsync(Message message);
    
    Task<List<Message>> GetMessagesAsync(Guid conversationId);
}