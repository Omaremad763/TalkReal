using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.Contracts.IRepo;

using Domain.Entities;

using Infra.Presistence;

using Microsoft.AspNetCore.Identity;

namespace Infra.Contracts_Imp;
public class UnitOfWork(ApplicationDbContext context, UserManager<User> _userManage) : IUnitofWork
{
    public IUserRepo UserRepo =>  new UserRepo(_userManage, context);

    public async Task<int> CommitAsync()
    {
       return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose(); GC.SuppressFinalize(this);
    }
}
