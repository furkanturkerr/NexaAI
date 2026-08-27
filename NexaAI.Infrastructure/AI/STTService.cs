// NexaAI.Infrastructure/AI/STTService.cs

using Microsoft.Extensions.Configuration;
using NexaAI.Application.Interfaces.Services;
using OpenAI.Audio;

namespace NexaAI.Infrastructure.AI;

public class STTService : ISTTService
{
    private readonly AudioClient _audioClient;

    public STTService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        var model = configuration["OpenAI:STTModel"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("OpenAI API Key bulunamadı.");

        if (string.IsNullOrWhiteSpace(model))
            throw new Exception("OpenAI STT model bilgisi bulunamadı.");

        _audioClient = new AudioClient(model, apiKey);
    }

    public async Task<string> TranscribeAsync(Stream audioStream, string fileName, CancellationToken cancellationToken = default)
    {
        var options = new AudioTranscriptionOptions
        {
            Language = "tr",
            ResponseFormat = AudioTranscriptionFormat.Simple
        };

        AudioTranscription transcription = await _audioClient.TranscribeAudioAsync(audioStream, fileName, options, cancellationToken);

        return transcription.Text;
    }
}