using MediatR;

namespace Domain.Events;

public record MessageCreatedEvent : INotification
{
    public Guid MessageId { get; init; }
    public string SenderId { get; init; }
    public string ReceiverId { get; init; }
    public string AttachmentURL { get; init; }
    public string Content { get; init; }
    public DateTime CreatedAt { get; init; }

    public MessageCreatedEvent(Guid messageId, string senderId, string receiverId, string content, string? url)
    {
        MessageId = messageId;
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
        AttachmentURL = url ?? "No Attachment";
    }
}