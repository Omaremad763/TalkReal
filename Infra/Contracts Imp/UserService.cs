using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.Contracts.IService;
using Application.DTOS;

using Domain.Entities;

using Microsoft.IdentityModel.Tokens;

namespace Infra.Contracts_Imp;
public class UserService(IUnitofWork unitofwork) : IUserService
{
    private static string GenerateJwt(User user)
    {
        var claims = new List<Claim>
        {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new (ClaimTypes.Email, user.Email!),
         };

        var Issuer = Environment.GetEnvironmentVariable("SaasJWTIssuer");
        var audience = Environment.GetEnvironmentVariable("SaasJWTAudience");
        var JWTkey = Environment.GetEnvironmentVariable("SaasJwtKey");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTkey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<string> RegistertUser(RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,

        };
        var result = await unitofwork.UserRepo.CreateUserWithRoleAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return "Failed Registration. Please try again later.";
        }

        return "User Registered Successfully";
    }
    public async Task<string> Login(LoginDto dto)
    {
        var user = await unitofwork.UserRepo.FindByEmailAsync(dto.Email);
        if (user == null || !await unitofwork.UserRepo.CheckPasswordAsync(user, dto.Password))
            throw new UnauthorizedAccessException();
        return GenerateJwt(user);

    }
}
