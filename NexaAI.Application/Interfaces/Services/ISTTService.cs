namespace NexaAI.Application.Interfaces.Services;

public interface ISTTService
{
    Task<string> TranscribeAsync(Stream audioStream, string fileName, CancellationToken cancellationToken = default);
}