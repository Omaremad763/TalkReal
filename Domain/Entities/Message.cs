using Domain.Enum;
using Domain.Value_Object;

namespace Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public required string SenderId { get; set; }
    public required string ReceiverId { get; set; }
    public required string Content { get; set; }
    public DateTime SentAt { get; set; }
    public MessageStatus Status { get; set; }
    public Attachment? Attachment { get; set; }
}