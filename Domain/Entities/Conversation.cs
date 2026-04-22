namespace Domain.Entities;
public class Conversation
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ParticipantAId { get; set; } = string.Empty;
    public string ParticipantBId { get; set; } = string.Empty;

    public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
}

