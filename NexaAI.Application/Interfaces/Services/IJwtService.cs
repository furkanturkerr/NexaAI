namespace NexaAI.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(string userId, string email, string name, string surname);
}