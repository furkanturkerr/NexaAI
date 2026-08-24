namespace NexaAI.Application.Features.Auth.Results;

public class GoogleUserResult
{
    public bool Succeeded { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? UserId { get; set; }

    public string? Email { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }
}