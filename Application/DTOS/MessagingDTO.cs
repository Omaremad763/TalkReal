using Domain.Enum;

using HotChocolate;

using Microsoft.AspNetCore.Http;

namespace Application.DTOS;
public class MessageDto
{
    public Guid Id { get; init; }
    public string SenderId { get; init; }
    public string ReceiverId { get; init; }
    public Guid ConversationId { get; init; }
    public string Content { get; init; }
    public DateTime SentAt { get; init; }
    public MessageStatus Status { get; init; }
    [GraphQLIgnore]
    public IFormFile? File { get; init; }
    public string? AttachmentUrl { get; init; }
    public MessageDto() { }

}

public record ChatHistoryRequestDto(
    string Senderid,
    string ReceiverId,
    int Take = 50
);