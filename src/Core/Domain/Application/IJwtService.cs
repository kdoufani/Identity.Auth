namespace Identity.Auth.Core.Domain.Application;

using Identity.Auth.Core.Domain.Dtos;
using Identity.Auth.Core.Domain.Entities;

public interface IJwtService
{
    Task<JwtTokenDto> GenerateJwtTokenAsync(ApplicationUser applicationUser, string refreshToken);
    Task<RefreshTokenDto> GenerateRefreshTokenAsync(Guid UserId, string? Token = null, CancellationToken cancellationToken = default);
}
