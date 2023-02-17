namespace Identity.Auth.Infrastructure.Data.Repositories;

using Identity.Auth.Core.Domain.Entities;
using Identity.Auth.Core.Domain.Repositories;
using Identity.Auth.Core.Infrastructure.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class JwtRepository : IJwtRepository
{
    private readonly IdentityContext _identityContext;

    public JwtRepository(IdentityContext identityContext)
    {
        _identityContext = identityContext;
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _identityContext.Set<RefreshToken>().AddAsync(refreshToken, cancellationToken);
        await _identityContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> FindRefreshToken(Expression<Func<RefreshToken, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _identityContext.Set<RefreshToken>()
             .FirstOrDefaultAsync(
                 predicate,
                 cancellationToken);
    }

    public async Task RemoveOldRefreshTokensAsync(Guid userId, long? ttlRefreshToken = null, CancellationToken cancellationToken = default)
    {
        var refreshTokens = _identityContext.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId);

        refreshTokens.ToList().RemoveAll(x => !x.IsRefreshTokenValid(ttlRefreshToken));

        await _identityContext.SaveChangesAsync(cancellationToken);
    }
}
