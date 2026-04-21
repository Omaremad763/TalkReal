using Application.CQRS;
using Application.DTOS;

using HotChocolate.Authorization;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MessagingController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Route("SendMessage")]
    public async Task<IActionResult> RegisterTenant([FromForm] MessageDto MessageDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await mediator.Send(new SendMessageCommand(MessageDto));
        var response = ApiResponse.Success(result);
        return Ok(response);
    }

}
