using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOS;
public class UpdateUserStatusDTO
{
    public Guid UserId { get; set; }
    public bool IsOnline { get; set; }
 };
public record UserStatusDto
{
    public UserStatusDto() { }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public bool IsOnline { get; init; }
    public DateTime LastSeen { get; init; }
    public string? ProfileImageUrl { get; set; }


}