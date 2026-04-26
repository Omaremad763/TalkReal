using Application.Contracts.IRepo;

using Domain.Entities;

using Infra.Migrations;
using Infra.Presistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infra.Contracts_Imp;
public class UserRepo(UserManager<User> _userManager, ApplicationDbContext Context) : IUserRepo
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

    public async Task<List<User>> GetUserStatus(CancellationToken CT)
    {
        return await Context.Users.AsNoTracking().ToListAsync(CT);
    }

    public async Task<bool> DeleteImagetById(Guid userid)

    {
        var getuser = await GetUserByidAsync(userid);
        if (getuser is null)
        {
            getuser.ProfileImageUrl = null;
            await Context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var user = await _userManager.Users
             .Include(u => u.RefreshTokens)
             .FirstOrDefaultAsync(u => u.RefreshTokens
            .Any(r => r.Token == refreshToken
              && r.Revoked == null
              && r.Expires > DateTime.UtcNow)
       );
        return user??null;
    }
}