using Application.Contracts;
using Application.Contracts.IRepo;

using Domain.Entities;

using Infra.Presistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infra.Contracts_Imp;
public class UnitOfWork(ApplicationDbContext context, UserManager<User> userManager) : IUnitofWork
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private bool _disposed;

    public IUserRepo UserRepo => new UserRepo(_userManager, _context);
    public IMessageRepo MessageRepo => new MessageRepo(_context);
    public IOutboxMessagesRepo OutboxMessagesRepo => new OutboxMessagesRepo(_context);
    public IConversationRepo ConversationRepo => new ConversationRepo(_context);

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _context.Dispose();
        }

        _disposed = true;
    }
}
