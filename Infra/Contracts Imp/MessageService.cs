using Application.Contracts;
using Application.Contracts.IService;
using Application.DTOS;

using Domain.Entities;
using Domain.Events;
using Domain.Value_Object;

using Newtonsoft.Json;

namespace Infra.Contracts_Imp;
public class MessageService(IUnitofWork unitOfWork,
    ICloudinaryService cloudinaryService) : IMessageService
{
    public async Task<bool> SendMessageAsync(MessageDto dto)
    {
        try
        {
            var p1 = string.Compare(dto.SenderId, dto.ReceiverId) < 0 ? dto.SenderId : dto.ReceiverId;
            var p2 = p1 == dto.SenderId ? dto.ReceiverId : dto.SenderId;

            var conversation = await unitOfWork.ConversationRepo.GetBetweenUsersAsync(p1, p2);

            if (conversation is null)
            {
                conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    ParticipantAId = p1,
                    ParticipantBId = p2,
                    CreatedAt = DateTime.UtcNow,
                    Title = $"Chat between {p1} and {p2}"
                };
                await unitOfWork.ConversationRepo.AddConversationAsync(conversation);
            }

            Attachment? attachment = null;
            if (dto.File != null && dto.File.Length > 0)
            {
                var uploadResult = await cloudinaryService.UploadFileAsync(dto.File);
                if (uploadResult != null)
                {
                    attachment = new Attachment(
                        Url: uploadResult.SecureUrl.ToString(),
                        Type: dto.File.ContentType,
                        Size: dto.File.Length,
                        PublicId: uploadResult.PublicId,
                        IsProcessed: false
                    );
                }
            }

            var message = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content ?? (attachment != null ? "Sent an attachment" : string.Empty),
                SentAt = DateTime.UtcNow,
                ConversationId = conversation.Id,
                Status = Domain.Enum.MessageStatus.Delivered,
                Attachment = attachment
            };

            await unitOfWork.MessageRepo.AddMessageAsync(message);

            var @event = new MessageCreatedEvent(
                message.Id,
                message.SenderId,
                message.ReceiverId,
                message.Content,
                message.Attachment?.Url
            );

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = typeof(MessageCreatedEvent).Name,
                Content = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                })
            };
            await unitOfWork.OutboxMessagesRepo.AddOutboxMessageAsync(outboxMessage);
            await unitOfWork.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public IQueryable<MessageDto> GetChatHistoryQuery(ChatHistoryRequestDto criteria)
    {
        var query = unitOfWork.MessageRepo.GetChatHistory(criteria);
        var mapping = query.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId,
            ConversationId = m.ConversationId,
            Content = m.Content,
            SentAt = m.SentAt,
            Status = m.Status,
            AttachmentUrl = m.Attachment.Url
        });
        return mapping;

    }

}
