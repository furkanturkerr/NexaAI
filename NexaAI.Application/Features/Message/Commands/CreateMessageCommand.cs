using MediatR;

namespace NexaAI.Application.Features.Message.Commands;

public class CreateMessageCommand : IRequest
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}