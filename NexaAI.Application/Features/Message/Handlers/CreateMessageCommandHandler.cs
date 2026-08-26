using System.Text;
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
    private readonly IRealtimeService _realtimeService;

    public CreateMessageCommandHandler(IMessageRepository messageRepository, IConversationRepository conversationRepository, IAIService aiService, IRealtimeService realtimeService)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _aiService = aiService;
        _realtimeService = realtimeService;
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
        
        //// OpenAI'dan parça parça gelen cevabı
        var fullResponse = new StringBuilder();

        await foreach (var chunk in _aiService.GetResponseStreamAsync(messages, cancellationToken))
        {
            // hepsi sırayla fullResponse'a eklenir.
            fullResponse.Append(chunk);
            
            // Aynı parçayı anında kullanıcının browser'ına gönderiyoruz.
            await _realtimeService.SendAIChunkAsync(request.UserId, request.ConversationId, chunk, cancellationToken);
        }

        var assistantMessage = new Domain.Entities.Message
        {
            ConversationId = request.ConversationId,
            Content = fullResponse.ToString(),
            Role = MessageRole.Assistant,
            CreatedAt = DateTime.UtcNow
        };
        
        await _messageRepository.CreateAsync(assistantMessage);
        
        conversation.UpdatedAt = DateTime.UtcNow;
        
        await _conversationRepository.UpdateAsync(conversation);
        
        // Browser'a stream'in bittiğini bildiriyoruz.
        await _realtimeService.SendAICompletedAsync(request.UserId, request.ConversationId, cancellationToken);
    }
}