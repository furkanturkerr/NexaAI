using MediatR;

namespace NexaAI.Application.Features.Conversation.Commands;

public class CreateConversationCommand : IRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = "Yeni Sohbet";
}