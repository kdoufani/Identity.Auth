namespace Identity.Auth.Core.Application.Services;

using Identity.Auth.Core.Domain.Application;
using Identity.Auth.Core.Domain.Dtos;
using Identity.Auth.Core.Domain.Entities;
using Identity.Auth.Core.Domain.Exceptions;
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

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        var identityUser = await _userManager.FindByNameAsync(loginDto.UserNameOrEmail) ??
                           await _userManager.FindByEmailAsync(loginDto.UserNameOrEmail);

        // instead of PasswordSignInAsync, we use CheckPasswordSignInAsync because we don't want set cookie, instead we use JWT
        var signinResult = await _signInManager.CheckPasswordSignInAsync(identityUser, loginDto.Password, false);

        if (signinResult.IsLockedOut)
        {
            throw new UserLockedException(identityUser.Id.ToString());
        }
        else if (!signinResult.Succeeded)
        {
            throw new PasswordIsInvalidException("Password is invalid.");
        }

        var refreshTokenDto = await _jwtService.GenerateRefreshTokenAsync(identityUser.Id);

        var accessTokenDto = await _jwtService.GenerateJwtTokenAsync(identityUser, refreshTokenDto.Token);

        _logger.LogInformation("User with ID: {ID} has been authenticated", identityUser.Id);

        return new LoginResponseDto()
        {
            UserId = identityUser.Id,
            FirstName = identityUser.FirstName,
            LastName = identityUser.LastName,
            Username = identityUser.UserName,
            AccessToken = accessTokenDto.Token,
            RefreshToken = refreshTokenDto.Token,
        };
    }
}
