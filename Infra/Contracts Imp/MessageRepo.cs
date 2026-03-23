using Application.Contracts.IRepo;
using Application.DTOS;

using Domain.Entities;

using Infra.Presistence;

using Microsoft.EntityFrameworkCore;

namespace Infra.Contracts_Imp;
public class MessageRepo(ApplicationDbContext context) : IMessageRepo
{
    public async Task<bool>AddMessageAsync(Message message)
    {
        await context.Messages.AddAsync(message);
        return true;
    }
    public IQueryable<Message> GetChatHistory(ChatHistoryRequestDto criteria)
    {
        return context.Messages
            .AsNoTracking()
            .Where(m =>
                (m.SenderId == criteria.UserId && m.ReceiverId == criteria.OtherUserId) ||
                (m.SenderId == criteria.OtherUserId && m.ReceiverId == criteria.UserId))
            .OrderByDescending(m => m.SentAt)
            .Take(criteria.Take);
    }
}