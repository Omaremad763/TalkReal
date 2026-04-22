using Application.Contracts.IRepo;
using Application.DTOS;

using Domain.Entities;

using Infra.Presistence;

using Microsoft.EntityFrameworkCore;

namespace Infra.Contracts_Imp;
public class MessageRepo(ApplicationDbContext context) : IMessageRepo
{
    public async Task<bool> AddMessageAsync(Message message)
    {
        await context.Messages.AddAsync(message);
        return true;
    }
    public IQueryable<Message> GetChatHistory(ChatHistoryRequestDto criteria)
    {
        return context.Messages
            .AsNoTracking()
            .Include(m => m.Attachment)
            .Where(m =>
                (m.SenderId == criteria.Senderid && m.ReceiverId == criteria.ReceiverId) ||
                (m.SenderId == criteria.ReceiverId && m.ReceiverId == criteria.Senderid))
            .OrderByDescending(m => m.SentAt)
            .Take(criteria.Take);
    }

    public async Task<Message?> GetMessageByPublicIdAsync(string publicId)
    {
        return await context.Messages.FirstOrDefaultAsync(m => m.Attachment != null && m.Attachment.PublicId == publicId);
    }

}