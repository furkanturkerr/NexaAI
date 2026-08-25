using Microsoft.EntityFrameworkCore;
using NexaAI.Application.Interfaces.Repositories;
using NexaAI.Domain.Entities;
using NexaAI.Infrastructure.Persistence.Context;

namespace NexaAI.Infrastructure.Persistence.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();   
    }

    public async Task<List<Message>> GetMessagesAsync(Guid conversationId)
    {
        var values = await _context.Messages
            .Where(x => x.Conversation.Id == conversationId)
            .ToListAsync();
        return values;
    }
}