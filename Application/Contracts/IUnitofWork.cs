using Application.Contracts.IRepo;

using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Contracts;
public interface IUnitofWork:IDisposable
{

    IUserRepo UserRepo { get; }
    IMessageRepo MessageRepo { get; } 
    IOutboxMessagesRepo OutboxMessagesRepo { get; }
    IConversationRepo ConversationRepo { get; }
    Task<int> CommitAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}

