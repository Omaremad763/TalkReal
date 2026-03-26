using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;

namespace Application.Contracts.IRepo;
public interface IUserRepo
{
    Task<IdentityResult> CreateUserAsync(User user, string password);

    Task<bool> CheckPasswordAsync(User user, string password);
    Task<User?> FindByEmailAsync(string Email);
    Task<User?> FindByidAsync(Guid Id);
    Task<List<User>> GetUserStatus(CancellationToken CT);
    Task<User?> GetUserByidAsync(Guid Id);
    Task<bool> DeleteImagetById(Guid userid);
}
