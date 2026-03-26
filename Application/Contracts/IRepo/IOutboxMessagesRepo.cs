using Domain.Entities;

namespace Application.Contracts.IRepo;

public interface IOutboxMessagesRepo
{
    Task AddOutboxMessageAsync(OutboxMessage message);
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, int maxErrors);
}