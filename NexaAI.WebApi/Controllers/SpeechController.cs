using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaAI.Application.Interfaces.Services;
using NexaAI.WebApi.Dtos.SpeechDtos;

namespace NexaAI.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SpeechController : ControllerBase
{
    private readonly ISTTService _sttService;
    private readonly ITTSService _ttsService;

    public SpeechController(ISTTService sttService, ITTSService ttsService)
    {
        _sttService = sttService;
        _ttsService = ttsService;
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
    
    [HttpPost("synthesize")]
    public async Task<IActionResult> Synthesize(SynthesizeSpeechRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new
            {
                Message = "Seslendirilecek metin boş olamaz."
            });
        }

        var audio = await _ttsService.GenerateSpeechAsync(request.Text, cancellationToken);

        return File(audio, "audio/mpeg");
    }
}