using MediatR;
using NexaAI.Application.Features.Conversation.Commands;
using NexaAI.Application.Features.Conversation.Results;
using NexaAI.Application.Interfaces.Repositories;

namespace NexaAI.Application.Features.Conversation.Handlers;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, CreateConversationResult>
{
    private readonly IConversationRepository _conversationRepository;

    public CreateConversationCommandHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<CreateConversationResult>  Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = new Domain.Entities.Conversation
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _conversationRepository.CreateAsync(conversation);
        
        return new CreateConversationResult
        {
            Id = conversation.Id,
            Title = conversation.Title
        };
    }
}