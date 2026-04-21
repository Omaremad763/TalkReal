using Application.CQRS;
using Application.DTOS;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WebhookController(IMediator mediator) : ControllerBase
{
    [HttpPost("CloudinaryCallback")]
    public async Task<ActionResult> CloudinaryCallback([FromBody] CloudinaryHookDTO DTO)
    {
        var result = await mediator.Send(new ProcessWebhookCommand(DTO));
        if (!result) return BadRequest("Attachment not found or message mismatch");
        var response = ApiResponse.Success(result);
        return Ok(response);
    }
}
