using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

namespace Application.Contracts.IService;
public interface IUserService
{
    Task<string> RegistertUser(RegisterDto dto);
    Task<string> Login(LoginDto dto);
    Task<bool> UpdateUserStatus(UpdateUserStatusDTO dto,CancellationToken CT);
    Task<List<UserStatusDto?>> GetUserStatus(CancellationToken CT);

}
