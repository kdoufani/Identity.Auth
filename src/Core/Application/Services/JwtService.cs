namespace Identity.Auth.Core.Application.Services;

using Identity.Auth.Core.Application.Security.Jwt;
using Identity.Auth.Core.Domain.Application;
using Identity.Auth.Core.Domain.Dtos;
using Identity.Auth.Core.Domain.Entities;
using Identity.Auth.Core.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Security.Claims;

public class JwtService : IJwtService
{
    private readonly IJwtRepository _jwtRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IJwtRepository jwtRepository,
        UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwtOptions, 
        ILogger<JwtService> logger)
    {
        _jwtRepository = jwtRepository;
        _userManager = userManager;
        _jwtOptions = jwtOptions;
        _logger = logger;
    }

    public async Task<JwtTokenDto> GenerateJwtTokenAsync(
        ApplicationUser applicationUser, 
        string refreshToken)
    {
        var identityUser = applicationUser;

        // authentication successful so generate jwt and refresh tokens
        var allClaims = await GetClaimsAsync(identityUser.UserName);
        var fullName = $"{identityUser.FirstName} {identityUser.LastName}";

        var tokenResult = JwtHandler.GenerateJwtToken(
            identityUser.UserName,
            identityUser.Email,
            identityUser.Id.ToString(),
            _jwtOptions.Value,
            fullName,
            refreshToken,
            allClaims.UserClaims.ToImmutableList(),
            allClaims.Roles.ToImmutableList(),
            allClaims.PermissionClaims.ToImmutableList());

        _logger.LogInformation("access-token generated, \n: {AccessToken}", tokenResult.Token);

        return new JwtTokenDto() { Token = tokenResult.Token , ExpireAt = tokenResult.ExpireAt };
    }

    public async Task<RefreshTokenDto> GenerateRefreshTokenAsync(Guid UserId, string? Token = null, CancellationToken cancellationToken = default)
    {
        Expression<Func<RefreshToken, bool>> predicate = (rt) => rt.UserId == UserId && rt.Token == Token;
        var refreshToken = await _jwtRepository.FindRefreshToken(predicate, cancellationToken);

        if (refreshToken == null)
        {
            var token = RefreshToken.GetRefreshToken();

            refreshToken = new RefreshToken
            {
                UserId = UserId,
                Token = token,
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddDays(1)
            };

            await _jwtRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);
        }
        else
        {
            var token = RefreshToken.GetRefreshToken();

            refreshToken.Token = token;
            refreshToken.ExpiredAt = DateTime.Now;
            refreshToken.CreatedAt = DateTime.Now.AddDays(10);

            await _jwtRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);
        }

        // remove old refresh tokens from user
        // we could also maintain them on the database with changing their revoke date
        await _jwtRepository.RemoveOldRefreshTokensAsync(UserId, cancellationToken: cancellationToken);

        return new RefreshTokenDto
        {
            Token = refreshToken.Token,
            CreatedAt = refreshToken.CreatedAt,
            ExpireAt = refreshToken.ExpiredAt,
            UserId = refreshToken.UserId,
            IsActive = refreshToken.IsActive,
            IsExpired = refreshToken.IsExpired,
            IsRevoked = refreshToken.IsRevoked,
            RevokedAt = refreshToken.RevokedAt
        };
    }

    private async Task<(IList<Claim> UserClaims, IList<string> Roles, IList<string> PermissionClaims)>
        GetClaimsAsync(string userName)
    {
        var appUser = await _userManager.FindByNameAsync(userName);
        var userClaims =
            (await _userManager.GetClaimsAsync(appUser)).Where(x => x.Type != CustomClaimTypes.Permission).ToList();
        var roles = await _userManager.GetRolesAsync(appUser);

        var permissions = (await _userManager.GetClaimsAsync(appUser))
            .Where(x => x.Type == CustomClaimTypes.Permission)?.Select(x => x
                .Value).ToList();

        return (UserClaims: userClaims, Roles: roles, PermissionClaims: permissions);
    }
}
