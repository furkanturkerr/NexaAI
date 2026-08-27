// NexaAI.WebUI/Controllers/SpeechController.cs

using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaAI.WebUI.Dtos.SpeechDtos;

namespace NexaAI.WebUI.Controllers;

[Authorize]
public class SpeechController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SpeechController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transcribe(IFormFile audio, CancellationToken cancellationToken)
    {
        if (audio == null || audio.Length == 0)
        {
            return BadRequest(new
            {
                Message = "Ses kaydı bulunamadı."
            });
        }

        var token = User.FindFirst("access_token")?.Value;

        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();

        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        await using var audioStream = audio.OpenReadStream();

        using var audioContent = new StreamContent(audioStream);

        audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse(audio.ContentType);
        using var formData = new MultipartFormDataContent();

        formData.Add(audioContent, "audio", audio.FileName);

        var response = await client.PostAsync("http://localhost:5015/api/Speech/transcribe", formData, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            return StatusCode((int)response.StatusCode, new { Message = error });
        }

        var result = await response.Content.ReadFromJsonAsync<TranscriptionResponseDto>(cancellationToken: cancellationToken);

        if (result == null)
        {
            return StatusCode(
                500,
                new
                {
                    Message = "STT cevabı okunamadı."
                });
        }

        return Ok(result);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Synthesize([FromBody] SynthesizeSpeechRequestDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Text))
            return BadRequest();

        var token =
            User.FindFirst("access_token")?.Value;

        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();

        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("http://localhost:5015/api/Speech/synthesize", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(
                (int)response.StatusCode);
        }

        var audio = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        return File(audio, "audio/mpeg");
    }
}