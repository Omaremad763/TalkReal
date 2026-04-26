using Application.CQRS;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost("AddPhoto/{userId}")]
    public async Task<ActionResult> AddPhoto([FromForm] IFormFile file, Guid userId)
    {
        var result = await mediator.Send(new AddUserPhotoCommand(userId, file));
        if (!result) return BadRequest("Failed to upload image");
        var response = ApiResponse.Success(result);
        return Ok(response);
    }

    [HttpGet]
    [Route("GetImageById/{id}")]
    public async Task<IActionResult> GeImageById(Guid id)
    {
        var result = await mediator.Send(new GetUserImageQuery(id));
        var response = ApiResponse.Success(result);
        return Ok(response);
    }
    [HttpDelete]
    [Route("DeleteImagetById/{id}")]
    public async Task<IActionResult> DeleteImagetById(Guid id)
    {
        var result = await mediator.Send(new DeleteImageByIdCommand(id));
        var response = ApiResponse.Success(result);
        return Ok(response);
    }


}

