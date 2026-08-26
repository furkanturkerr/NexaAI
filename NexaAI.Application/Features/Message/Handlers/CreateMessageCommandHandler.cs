using MediatR;
using NexaAI.Application.Features.Message.Commands;
using NexaAI.Application.Interfaces.Repositories;
using NexaAI.Application.Interfaces.Services;
using NexaAI.Domain.Enums;

namespace NexaAI.Application.Features.Message.Handlers;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IAIService _aiService;

    public CreateMessageCommandHandler(IMessageRepository messageRepository, IConversationRepository conversationRepository, IAIService aiService)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _aiService = aiService;
    }

    public async Task Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId);
        
        if (conversation == null)
            throw new Exception("Bu sohbete erişim yetkiniz yok veya sohbet bulunamadı");

        if (conversation.UserId != request.UserId)
            throw new Exception("Bu sohbete erişim yetkiniz yok veya sohbet bulunamadı");
        
        var message = new Domain.Entities.Message
        {
            ConversationId = request.ConversationId,
            Content = request.Content,
            Role = MessageRole.User,
            CreatedAt = DateTime.UtcNow
        };
        
        await _messageRepository.CreateAsync(message);
        
        var messages = await _messageRepository.GetMessagesAsync(request.ConversationId);

        var aiResponse = await _aiService.GetResponseAsync(messages);

        var assistantMessage = new Domain.Entities.Message
        {
            ConversationId = request.ConversationId,
            Content = aiResponse,
            Role = MessageRole.Assistant,
            CreatedAt = DateTime.UtcNow
        };
        
        await _messageRepository.CreateAsync(assistantMessage);
        
        conversation.UpdatedAt = DateTime.UtcNow;
        
        await _conversationRepository.UpdateAsync(conversation);
    }
}