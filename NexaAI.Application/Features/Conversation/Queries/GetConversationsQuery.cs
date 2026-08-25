using MediatR;
using NexaAI.Application.Features.Conversation.Results;

namespace NexaAI.Application.Features.Conversation.Queries;

public class GetConversationsQuery : IRequest<List<GetConversationResult>>
{
    public string UserId { get; set; }

    public GetConversationsQuery(string userId)
    {
        UserId = userId;
    }
}