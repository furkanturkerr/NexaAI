namespace NexaAI.Application.Features.Auth.Results;

public class LoginResult
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
}