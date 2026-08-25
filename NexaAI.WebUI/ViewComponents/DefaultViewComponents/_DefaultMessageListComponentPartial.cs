using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NexaAI.WebUI.Dtos.ConversationDtos;

namespace NexaAI.WebUI.ViewComponents.DefaultViewComponents;

public class _DefaultMessageListComponentPartial : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public _DefaultMessageListComponentPartial(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var token = ViewContext.HttpContext.User.FindFirst("access_token")?.Value;
        
        var client = _httpClientFactory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.GetAsync("http://localhost:5015/api/Conversation");

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadFromJsonAsync<List<ResultConversationDto>>();
            return View(jsonData);
        }
        
        return View();
    }
}