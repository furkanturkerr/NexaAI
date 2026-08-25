using NexaAI.Domain.Enums;

namespace NexaAI.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public string Content { get; set; } = string.Empty;

    public MessageRole Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public Conversation Conversation { get; set; } = null!;
}