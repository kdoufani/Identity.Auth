namespace Identity.Auth.Core.Application.Services;

using Identity.Auth.Core.Domain.Application;
using Identity.Auth.Core.Domain.Dtos;
using Identity.Auth.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtService _jwtService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IJwtService jwtService,
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        ILogger<AuthenticationService> logger)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public ILogger<AuthenticationService> Logger { get; }

    public async Task<LoginResponseDto> LoginAsync(string userNameOrEmail, string password)
    {
        var identityUser = await _userManager.FindByNameAsync(userNameOrEmail) ??
                           await _userManager.FindByEmailAsync(userNameOrEmail);

        // instead of PasswordSignInAsync, we use CheckPasswordSignInAsync because we don't want set cookie, instead we use JWT
        var signinResult = await _signInManager.CheckPasswordSignInAsync(identityUser, password, false);

        var refreshTokenDto = await _jwtService.GenerateRefreshTokenAsync(identityUser.Id);

        var accessTokenDto = await _jwtService.GenerateJwtTokenAsync(identityUser, refreshTokenDto.Token);

        _logger.LogInformation("User with ID: {ID} has been authenticated", identityUser.Id);

        return new LoginResponseDto(identityUser, accessTokenDto.Token, refreshTokenDto.Token);
    }
}
