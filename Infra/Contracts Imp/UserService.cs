using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Application.Contracts;
using Application.Contracts.IService;
using Application.DTOS;

using Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infra.Contracts_Imp;
public class UserService(IUnitofWork unitofwork, IConfiguration config) : IUserService
{
    public async Task RegistertUser(RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.Username,
            Email = dto.Email,
        };
        var result = await unitofwork.UserRepo.CreateUserAsync(user, dto.Password);
        if (result.Errors.Any())
        {
            var errorMessages = string.Join(", ",
                result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration Failed: {errorMessages}");
        }
    }
    public async Task<string> Login(LoginDto dto)
    {
        var user = await unitofwork.UserRepo.FindByEmailAsync(dto.Email);
        if (user is null || !await unitofwork.UserRepo.CheckPasswordAsync(user, dto.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");
        return GenerateJwt(user);
    }
    public async Task<bool> UpdateUserStatus(UpdateUserStatusDto dto, CancellationToken CT)
    {
        var user = await unitofwork.UserRepo.FindByidAsync(dto.UserId);
        if (user == null) return false;
        if (user != null)
        {
            user.IsOnline = dto.IsOnline;
            user.LastSeen = DateTime.UtcNow;
        }
        return await unitofwork.CommitAsync() > 0;
    }
    public async Task<List<UserStatusDto?>> GetUserStatus(CancellationToken CT)
    {
        var entity = await unitofwork.UserRepo.GetUserStatus(CT);
        var mapping = new List<UserStatusDto>();
        if (entity != null)
        {
            foreach (var user in entity)
            {
                mapping.Add(new UserStatusDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    IsOnline = user.IsOnline,
                    LastSeen = user.LastSeen,
                    ProfileImageUrl = user.ProfileImageUrl,
                });
            }
            return mapping;
        }
        return null;
    }
    public async Task<bool> UpdateProfileImageAsync(Guid id, string imageUrl)
    {
        var user = await unitofwork.UserRepo.FindByidAsync(id);
        if (user == null)
            return false;

        user.ProfileImageUrl = imageUrl;
        await unitofwork.CommitAsync();
        return true;
    }
    public async Task<User?> GetUserByidAsync(Guid Id)
    {
        return await unitofwork.UserRepo.GetUserByidAsync(Id);
    }

    public async Task<bool> DeleteImagetById(Guid userid)
    {
        return await unitofwork.UserRepo.DeleteImagetById(userid);
    }
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
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
