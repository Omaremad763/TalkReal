using Application.DTOS;

using Domain.Entities;

namespace Application.Contracts.IRepo;
public interface IMessageRepo
{
    Task<bool> AddMessageAsync(Message message);
    IQueryable<Message> GetChatHistory(ChatHistoryRequestDto criteria);
    Task<Message?> GetMessageByPublicIdAsync(string publicId);
}
