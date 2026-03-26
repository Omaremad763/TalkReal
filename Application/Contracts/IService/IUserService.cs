using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

using Domain.Entities;

namespace Application.Contracts.IService;
public interface IUserService
{
    Task<string> RegistertUser(RegisterDto dto);
    Task<string> Login(LoginDto dto);
    Task<bool> UpdateUserStatus(UpdateUserStatusDTO dto,CancellationToken CT);
    Task<List<UserStatusDto?>> GetUserStatus(CancellationToken CT);
    Task<bool> UpdateProfileImageAsync(Guid id, string imageUrl);
    Task<User?> GetUserByidAsync(Guid Id);

    Task<bool> DeleteImagetById(Guid userid);
}
