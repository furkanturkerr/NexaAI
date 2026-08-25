using MediatR;
using NexaAI.Application.Features.Conversation.Queries;
using NexaAI.Application.Features.Conversation.Results;
using NexaAI.Application.Interfaces.Repositories;

namespace NexaAI.Application.Features.Conversation.Handlers;

public class GetUserConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<GetConversationResult>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetUserConversationsQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<List<GetConversationResult>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var values = await _conversationRepository.GetConversationsAsync(request.UserId);
        return values.Select(x => new GetConversationResult
        {
            Id = x.Id,
            Title = x.Title,
            UpdatedAt = x.UpdatedAt
        }).ToList();
    }
}