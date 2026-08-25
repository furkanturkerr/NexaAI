namespace NexaAI.WebUI.Dtos.ConversationDtos;

public class CreateMessageDto
{
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid ConversationId { get; set; }
}