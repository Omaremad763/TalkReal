using Application.Contracts;
using Application.Contracts.IRepo;

using Domain.Entities;

using Infra.Presistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Contracts_Imp;
public class UnitOfWork(ApplicationDbContext context, UserManager<User> _userManage) : IUnitofWork 
{
    public IUserRepo UserRepo =>  new UserRepo(_userManage, context);

    public IMessageRepo MessageRepo =>  new MessageRepo(context);

    public IOutboxMessagesRepo OutboxMessagesRepo =>  new OutboxMessagesRepo(context);

    public IConversationRepo ConversationRepo =>  new ConversationRepo(context);

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await context.Database.BeginTransactionAsync();
    }
    public async Task<int> CommitAsync()
    {
       return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose(); GC.SuppressFinalize(this);
    }
}
