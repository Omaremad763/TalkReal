using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Entities;
using Domain.Enum;

using HotChocolate;

using Microsoft.AspNetCore.Http;

namespace Application.DTOS;
public class MessageDto
{
public MessageDto() { }
public Guid Id { get; init; }
public string SenderId { get; init; }
public string ReceiverId { get; init; }
public Guid ConversationId { get; init; }
public string Content { get; init; }
public DateTime SentAt { get; init; }
public MessageStatusEnum Status { get; init; }
[GraphQLIgnore]
 public IFormFile? File { get; init; }
 public string? attachmentUrl { get; init; }
}

public record ChatHistoryRequestDto(
    string senderid,
    string receiverId,
    int Take = 50
);