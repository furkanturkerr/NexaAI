using MediatR;

namespace NexaAI.Application.Features.Conversation.Commands;

public class DeleteConversationCommand : IRequest
{
    public Guid ConversationId { get; set; }

    public DeleteConversationCommand(Guid conversationId)
    {
        ConversationId = conversationId;
    }
}