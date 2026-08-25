using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NexaAI.WebUI.Dtos.ConversationDtos;

namespace NexaAI.WebUI.Controllers;

public class MessageController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MessageController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // GET
    public async Task<IActionResult> Send(CreateMessageDto dto)
    {
        var token = User.FindFirst("access_token")?.Value;

        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();

        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await client.PostAsJsonAsync("http://localhost:5015/api/Messages", dto);

        return RedirectToAction("Index", "Default",
            new { conversationId = dto.ConversationId });
    }
}