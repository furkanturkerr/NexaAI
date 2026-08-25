using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NexaAI.Domain.Entities;
using NexaAI.Infrastructure.Identity;

namespace NexaAI.Infrastructure.Persistence.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Message> Messages { get; set; }
}