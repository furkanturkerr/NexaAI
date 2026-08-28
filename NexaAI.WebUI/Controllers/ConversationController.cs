using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaAI.WebUI.Dtos.ConversationDtos;

namespace NexaAI.WebUI.Controllers;

[AutoValidateAntiforgeryToken]

public class ConversationController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ConversationController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateConversationDto dto)
    {
        var token = User.FindFirst("access_token")?.Value;
        
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();
        
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.PostAsJsonAsync("http://localhost:5015/api/Conversation", dto);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Sohbet oluşturulmadı!");
        }
        
        return RedirectToAction("Index", "Default");
    }
    
    [HttpPost]
    public async Task<IActionResult> Start(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return BadRequest();

        var token = User.FindFirst("access_token")?.Value;

        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            "http://localhost:5015/api/Conversation",
            new { Title = content });

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var conversation = await response.Content.ReadFromJsonAsync<CreateConversationResultDto>();

        if (conversation == null)
            return StatusCode(500);

        return Ok(new { conversationId = conversation.Id });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var token = User.FindFirst("access_token")?.Value;
        
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();
        
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.DeleteAsync($"http://localhost:5015/api/Conversation/{id}");

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Sohbet silinmedi!");
        }
        
        return RedirectToAction("Index", "Default");
    }
}