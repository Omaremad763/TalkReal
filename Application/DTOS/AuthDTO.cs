namespace Application.DTOS;
public record RegisterDto(
    string Username,
    string Email,
    string Password
);

public record LoginDto(
    string Email,
    string Password
);

public class TokenDto
{
   public  string AccessToken { get; set; }
   public string RefreshToken { get; set; }

}