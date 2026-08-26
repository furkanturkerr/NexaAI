namespace NexaAI.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = "Yeni Sohbet";

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}