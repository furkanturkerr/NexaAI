namespace NexaAI.Application.Interfaces.Services;

public interface ITTSService
{
    Task<byte[]> GenerateSpeechAsync(string text, CancellationToken cancellationToken = default);
}