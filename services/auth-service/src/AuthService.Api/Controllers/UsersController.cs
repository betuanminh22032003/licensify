using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AuthService.Application.Features.User.Queries.Get;
using AuthService.Application.Features.User.Queries.GetAll;
using AuthService.Application.Features.User.Commands.Update;
using AuthService.Application.Features.User.Commands.Delete;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetUserQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (result.IsSuccess)
        {
            return Ok(result.User);
        }

        return NotFound(result.Message);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetUsersQuery();
        var result = await _mediator.Send(query);
        
        return Ok(result.Users);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("User ID mismatch");
        }

        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(result.User);
        }

        return BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteUserCommand { Id = id };
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
        {
            return Ok(new { message = "User deleted successfully" });
        }

        return BadRequest(result.Message);
    }
}
