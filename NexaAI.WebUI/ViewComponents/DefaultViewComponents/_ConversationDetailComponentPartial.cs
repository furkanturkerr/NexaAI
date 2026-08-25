using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using NexaAI.WebUI.Dtos.ConversationDtos;

namespace NexaAI.WebUI.ViewComponents.DefaultViewComponents;

public class _ConversationDetailComponentPartial : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public _ConversationDetailComponentPartial(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync(Guid conversationId)
    {
        var token = ViewContext.HttpContext.User.FindFirst("access_token")?.Value;
        
        var client = _httpClientFactory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.GetAsync($"http://localhost:5015/api/Messages/{conversationId}");

        var conversation = new ConversationDetailDto
        {
            Id = conversationId,
            Messages = new List<ResultMessageDto>()
        };

        if (response.IsSuccessStatusCode)
        {
            var messages = await response.Content.ReadFromJsonAsync<List<ResultMessageDto>>();

            conversation.Messages = messages ?? new List<ResultMessageDto>();
        }

        return View(conversation);
    }
}