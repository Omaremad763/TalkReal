using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> RegisterTenant([FromBody] RegisterDto registrationDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await mediator.Send(new RegisterCommand(registrationDto));
        var response = ApiResponse.Success();
        return Ok(response);
    }
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await mediator.Send(new LoginCommand(dto));
        SetRefreshTokenInCookie(result.RefreshToken);
        var response = ApiResponse.Success(result);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("No Refresh Token provided.");

        var result = await mediator.Send(new GenerateRefreshTokenCommand(refreshToken));

        if (result is null)return Unauthorized("Invalid Session.");

        SetRefreshTokenInCookie(result.RefreshToken);

        return Ok(new { accessToken = result.AccessToken });
    }

    private void SetRefreshTokenInCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,     
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/"
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

}


