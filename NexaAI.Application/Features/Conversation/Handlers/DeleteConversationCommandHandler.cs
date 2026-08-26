using MediatR;
using NexaAI.Application.Features.Conversation.Commands;
using NexaAI.Application.Interfaces.Repositories;

namespace NexaAI.Application.Features.Conversation.Handlers;

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand>
{
    private readonly IConversationRepository _conversationRepository;

    public DeleteConversationCommandHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        var value = await _conversationRepository.GetByIdAsync(request.ConversationId);
        await _conversationRepository.DeleteAsync(value);
    }
}