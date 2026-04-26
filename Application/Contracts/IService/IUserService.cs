using Application.DTOS;

using Domain.Entities;

namespace Application.Contracts.IService;
public interface IUserService
{
    Task RegistertUser(RegisterDto dto);
    Task<bool> UpdateUserStatus(UpdateUserStatusDto dto, CancellationToken CT);
    Task<List<UserStatusDto>> GetUserStatus(CancellationToken CT);
    Task<bool> UpdateProfileImageAsync(Guid id, string imageUrl);
    Task<User?> GetUserByidAsync(Guid Id);

    Task<bool> DeleteImagetById(Guid userid);
}
