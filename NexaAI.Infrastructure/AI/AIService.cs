using Microsoft.Extensions.Configuration;
using NexaAI.Application.Interfaces.Services;
using NexaAI.Domain.Entities;
using NexaAI.Domain.Enums;
using OpenAI.Chat;

namespace NexaAI.Infrastructure.AI;

public class AIService : IAIService
{
    private readonly ChatClient _client;

    public AIService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        var model = configuration["OpenAI:Model"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("OpenAI API Key bulunamadı.");

        if (string.IsNullOrWhiteSpace(model))
            throw new Exception("OpenAI model bilgisi bulunamadı.");

        _client = new ChatClient(model, apiKey);
    }

    public async IAsyncEnumerable<string> GetResponseStreamAsync(
        List<Message> messages,
        CancellationToken cancellationToken = default)
    {
        var chatMessages = new List<ChatMessage>
        {
            new SystemChatMessage(
                """
                Sen NexaAI isimli bir yapay zeka asistanısın.
                
                Yanıtlarını geçerli Markdown formatında oluştur.
                
                FORMAT KURALLARI:
                
                - Normal açıklamalar normal Markdown paragrafı olarak yazılmalı.
                - Başlık gerekiyorsa Markdown başlıkları kullan.
                - Listelerde Markdown madde veya numaralı liste kullan.
                - Kod olmayan metni kod bloğuna koyma.
                - Kod içeren HER örnek mutlaka üçlü backtick kod bloğu içinde olmalı.
                - Kod bloğunda mümkünse dil adı belirtilmeli. Örneğin: csharp, json, bash.
                - using satırları dahil hiçbir kod satırını normal paragraf içinde yazma.
                - Metot imzasını kod bloğu dışında bırakma.
                - Açılan ve kapanan süslü parantezleri kod bloğu dışında bırakma.
                - Tek bir kod örneğini birden fazla kod bloğuna bölme.
                - Kod bloğundan önce ve sonra boş satır bırak.
                
                ```csharp
                using System.IdentityModel.Tokens.Jwt;
                using System.Security.Claims;
                
                public string CreateToken()
                {
                    return "token";
                }
                ```
                
                Koddan sonra açıklamaya normal Markdown paragrafı ile devam et.
                """
            )
        };

        // DB'deki sohbet geçmişini OpenAI mesajlarına dönüştürüyoruz.
        foreach (var message in messages)
        {
            if (message.Role == MessageRole.User)
            {
                chatMessages.Add(
                    new UserChatMessage(message.Content));
            }
            else if (message.Role == MessageRole.Assistant)
            {
                chatMessages.Add(
                    new AssistantChatMessage(message.Content));
            }
        }

        // Artık tam cevabı beklemiyoruz.
        // OpenAI cevabı parça parça stream olarak gönderiyor.
        var completionUpdates = _client.CompleteChatStreamingAsync(chatMessages, cancellationToken: cancellationToken);

        // OpenAI'dan yeni parça geldikçe çalışır.
        await foreach (var update in completionUpdates)
        {
            foreach (var content in update.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(content.Text))
                {
                    // Tam cevabı beklemeden parçayı dışarı veriyoruz.
                    yield return content.Text;
                }
            }
        }
    }
}