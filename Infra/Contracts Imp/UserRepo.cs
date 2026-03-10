using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts.IRepo;

using Domain.Entities;

using Infra.Presistence;

using Microsoft.AspNetCore.Identity;

namespace Infra.Contracts_Imp;
public class UserRepo(UserManager<User> _userManager,ApplicationDbContext Context) : IUserRepo
{
    public async Task<IdentityResult> CreateUserWithRoleAsync(User user, string password)
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


}