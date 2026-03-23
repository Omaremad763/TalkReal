using Application.Contracts.IRepo;

using Domain.Entities;

using Infra.Presistence;

using Microsoft.EntityFrameworkCore;

namespace Infra.Contracts_Imp;
public class OutboxMessagesRepo(ApplicationDbContext context) : IOutboxMessagesRepo
{
    public async Task AddOutboxMessageAsync(OutboxMessage message)
    {
        await context.Set<OutboxMessage>().AddAsync(message);
    }

    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, int maxErrors)
    {
        return await context.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null && m.ErrorCount < maxErrors)
            .OrderBy(m => m.OccurredOnUtc)    
            .Take(batchSize)
            .ToListAsync();
    }
}