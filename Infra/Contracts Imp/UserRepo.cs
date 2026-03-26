using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.IRepo;
using Application.DTOS;

using Domain.Entities;

using Infra.Presistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infra.Contracts_Imp;
public class UserRepo(UserManager<User> _userManager,ApplicationDbContext Context) : IUserRepo
{
    public async Task<IdentityResult> CreateUserAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        return result;
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }
    public async Task<User?> FindByEmailAsync(string Email)
    {
        return await _userManager.FindByEmailAsync(Email);
    }

    public async Task<User?> FindByidAsync(Guid Id)
    {
        return await Context.Users.FindAsync(Id);
    }
    public async Task<User?> GetUserByidAsync(Guid Id)
    {
        return await Context.Users.FirstOrDefaultAsync(x => x.Id == Id);
    }

    public async Task<List<User>> GetUserStatus( CancellationToken CT)
    {
        return await Context.Users.AsNoTracking().ToListAsync(CT);
    }

    public async Task<bool> DeleteImagetById(Guid userid)
    {
        var getuser = await GetUserByidAsync(userid);
        if (getuser != null) 
        { 
            getuser.ProfileImageUrl = null;
            Context.SaveChanges();
            return true;
        };
        return false;
    }
}