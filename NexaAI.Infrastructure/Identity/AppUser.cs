using Microsoft.AspNetCore.Identity;

namespace NexaAI.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}