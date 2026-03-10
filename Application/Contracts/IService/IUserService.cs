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

}
