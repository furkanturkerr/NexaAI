using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaAI.Application.Interfaces.Services;

namespace NexaAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SpeechController : ControllerBase
{
    private readonly ISTTService _sttService;

    public SpeechController(ISTTService sttService)
    {
        _sttService = sttService;
    }

    [HttpPost("transcribe")]
    public async Task<IActionResult> Transcribe(IFormFile audio, CancellationToken cancellationToken)
    {
        if (audio == null || audio.Length == 0)
        {
            return BadRequest(new
            {
                Message = "Ses dosyası bulunamadı."
            });
        }

        await using var audioStream = audio.OpenReadStream();

        var text = await _sttService.TranscribeAsync(audioStream, audio.FileName, cancellationToken);

        return Ok(new
        {
            Text = text
        });
    }
}