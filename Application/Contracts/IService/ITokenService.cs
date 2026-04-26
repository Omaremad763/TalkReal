using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOS;

namespace Application.Contracts.IService;
public interface ITokenService
{
    Task<TokenDto> Login(LoginDto dto);
    Task<TokenDto> GetRefreshTokenAsync(string refreshToken);
}
