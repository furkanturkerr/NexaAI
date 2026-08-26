using Microsoft.EntityFrameworkCore;
using NexaAI.Application.Interfaces.Repositories;
using NexaAI.Domain.Entities;
using NexaAI.Infrastructure.Persistence.Context;

namespace NexaAI.Infrastructure.Persistence.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _context;

    public ConversationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Conversation conversation)
    {
        await _context.Conversations.AddAsync(conversation);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Conversation>> GetConversationsAsync(string userId)
    {
        var values = await _context.Conversations
            .Where(x => x.UserId == userId)
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync();
        return values;
    }

    public async Task<Conversation?> GetByIdAsync(Guid id)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(Conversation conversation)
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Conversation conversation)
    {
        conversation.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}