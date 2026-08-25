using MediatR;
using NexaAI.Application.Features.Message.Queries;
using NexaAI.Application.Features.Message.Results;
using NexaAI.Application.Interfaces.Repositories;

namespace NexaAI.Application.Features.Message.Handlers;

public class GetMessageQueryHandler : IRequestHandler<GetMessageQuery, List<GetMessageResult>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;

    public GetMessageQueryHandler(IMessageRepository messageRepository, IConversationRepository conversationRepository)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
    }

    public async Task<List<GetMessageResult>> Handle(GetMessageQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId);
        
        if (conversation == null)
            throw new Exception("Bu sohbete erişim yetkiniz yok veya sohbet bulunamadı");

        if (conversation.UserId != request.UserId)
            throw new Exception("Bu sohbete erişim yetkiniz yok veya sohbet bulunamadı");
        
        var values = await _messageRepository.GetMessagesAsync(request.ConversationId);
        return values.Select(x=> new GetMessageResult()
        {
            Content = x.Content,
            Role = x.Role
        }).ToList();
    }
}