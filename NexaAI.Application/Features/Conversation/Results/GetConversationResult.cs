namespace NexaAI.Application.Features.Conversation.Results;

public class GetConversationResult
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}