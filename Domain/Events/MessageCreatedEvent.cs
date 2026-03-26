using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace Domain.Events;

public record MessageCreatedEvent : INotification       
{
    public Guid MessageId { get; init; }
    public string SenderId { get; init; }
    public string ReceiverId { get; init; }
    public string Content { get; init; }
    public DateTime CreatedAt { get; init; }

    public MessageCreatedEvent(Guid messageId, string senderId, string receiverId, string content)
    {
        MessageId = messageId;
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        CreatedAt = DateTime.UtcNow;        
    }
}