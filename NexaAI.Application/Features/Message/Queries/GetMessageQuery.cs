using MediatR;
using NexaAI.Application.Features.Message.Results;

namespace NexaAI.Application.Features.Message.Queries;

public class GetMessageQuery : IRequest<List<GetMessageResult>>
{
    public Guid ConversationId { get; set; }
    public string UserId { get; set; } = string.Empty;
}