namespace Identity.Auth.Api.Controllers.v1;

using BuildingBlocks.Core.Securities;
using BuildingBlocks.Core.Startup.Attributes;
using Identity.Auth.Api.Contracts.v1.Response;
using Identity.Auth.Core.Domain.Application;
using Identity.Auth.Core.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Version("1")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        ArgumentNullException.ThrowIfNull(userService);

        _userService = userService;
    }

    [ProducesResponseType(typeof(Response<IdentityUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(new Response<IdentityUserDto>(user));
    }

    [ProducesResponseType(typeof(Response<IdentityUserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserDto userDto)
    {
        var user = await _userService.RegisterUserAsync(userDto);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new Response<IdentityUserDto>(user));
    }
}
