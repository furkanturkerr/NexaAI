namespace NexaAI.WebUI.Dtos.AccountDtos;

public class AuthResponseDto
{
    public bool Succeeded { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
}