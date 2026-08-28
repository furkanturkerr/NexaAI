using MediatR;
using NexaAI.Application.Features.Conversation.Results;

namespace NexaAI.Application.Features.Conversation.Commands;

public class CreateConversationCommand : IRequest<CreateConversationResult>
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = "Yeni Sohbet";
}