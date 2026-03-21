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
            var result = await mediator.Send(new RegisterCommand(registrationDto));
            var response = ApiResponse.Success(result);
            return Ok(response);
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await mediator.Send(new LoginCommand(dto));
            var response = ApiResponse.Success(result);
            return Ok(response);
        }
}

