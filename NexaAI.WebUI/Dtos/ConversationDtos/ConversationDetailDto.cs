namespace NexaAI.WebUI.Dtos.ConversationDtos;

public class ConversationDetailDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<ResultMessageDto> Messages { get; set; } = new();
}