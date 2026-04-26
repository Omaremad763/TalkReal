using Domain.Entities;

using HotChocolate;

namespace Application.DTOS;
public class UpdateUserStatusDto
{
    public Guid UserId { get; set; }
    public bool IsOnline { get; set; }
};
public record UserStatusDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public bool IsOnline { get; init; }
    public DateTime LastSeen { get; init; }
    public string? ProfileImageUrl { get; set; }
    public UserStatusDto() { }

}