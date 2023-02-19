namespace Identity.Auth.Api.Controllers.v1;

using BuildingBlocks.Core.Startup.Attributes;
using Identity.Auth.Api.Contracts.v1.Response;
using Identity.Auth.Core.Domain.Application;
using Identity.Auth.Core.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Version("1")]
public class AuthenticationController : Controller
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        ArgumentNullException.ThrowIfNull(authenticationService);

        _authenticationService = authenticationService;
    }

    [ProducesResponseType(typeof(Response<LoginResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto loginDto)
    {
        var user = await _authenticationService.LoginAsync(loginDto);
        return Ok(new Response<LoginResponseDto>(user));
    }
}
