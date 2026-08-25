using NexaAI.Domain.Enums;

namespace NexaAI.Application.Features.Message.Results;

public class GetMessageResult
{
    public string Content { get; set; } = string.Empty;
    public MessageRole Role { get; set; }
}