using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Application.Contracts;
using Application.Contracts.IService;
using Application.DTOS;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infra.Contracts_Imp;
public class TokenService(
    IUnitofWork unitofwork
    ,IConfiguration config
    ) :ITokenService
{
    private  string GenerateJwt(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Email, user.Email!),
            new (ClaimTypes.Name, user.UserName!),
         };
        var Issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];
        var JWTkey = config["Jwt:key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTkey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(2),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private async Task<TokenDto> GenerateAndSaveTokens(User user)
    {
        var accessToken = GenerateJwt(user);
        var newRefreshToken = CreateRefreshToken(user.Id);

        user.RefreshTokens.Add(newRefreshToken);
        await unitofwork.CommitAsync();

        return new TokenDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token
        };
    }

    private static RefreshToken CreateRefreshToken(Guid userId)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            UserId = userId
        };
    }

    public async Task<TokenDto> Login(LoginDto dto)
    {
        var user = await unitofwork.UserRepo.FindByEmailAsync(dto.Email);

        if (user is null || !await unitofwork.UserRepo.CheckPasswordAsync(user, dto.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateAndSaveTokens(user);
    }
    public async Task<TokenDto> GetRefreshTokenAsync(string refreshToken)
    {
        var user = await unitofwork.UserRepo.GetUserByRefreshTokenAsync(refreshToken) ?? throw new UnauthorizedAccessException("Invalid session.");
        var oldToken = user.RefreshTokens.Single(t => t.Token == refreshToken);

        oldToken.Revoked = DateTime.UtcNow;

        return await GenerateAndSaveTokens(user);
    }
}

