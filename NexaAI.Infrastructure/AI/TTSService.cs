using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using NexaAI.Application.Interfaces.Services;

namespace NexaAI.Infrastructure.AI;

public class TTSService : ITTSService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly string _voice;

    public TTSService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["OpenAI:ApiKey"]
                  ?? throw new Exception(
                      "OpenAI API Key bulunamadı.");

        _model = configuration["OpenAI:TTSModel"]
                 ?? throw new Exception(
                     "OpenAI TTS model bilgisi bulunamadı.");

        _voice = configuration["OpenAI:TTSVoice"]
                 ?? throw new Exception(
                     "OpenAI TTS voice bilgisi bulunamadı.");
    }

    public async Task<byte[]> GenerateSpeechAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new Exception(
                "Seslendirilecek metin boş olamaz.");

        var request = new
        {
            model = _model,
            input = text,
            voice = _voice,
            response_format = "mp3",

            instructions =
                """
                Türkçe konuş.
                Sakin, sıcak ve arkadaşça bir ton kullan.
                Yumuşak bir sesle konuş.
                Acele etme ve kelimeleri doğal şekilde telaffuz et.
                Agresif, sert, otoriter veya aşırı enerjik konuşma.
                Bir arkadaşına açıklama yapıyormuş gibi rahat konuş.
                """
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/speech");

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        httpRequest.Content = JsonContent.Create(request);

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new Exception($"OpenAI TTS hatası ({(int)response.StatusCode}): {error}");
        }

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}