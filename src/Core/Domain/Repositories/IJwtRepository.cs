using Identity.Auth.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Identity.Auth.Core.Domain.Repositories;

public interface IJwtRepository
{
    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RemoveOldRefreshTokensAsync(Guid userId, long? ttlRefreshToken = null, CancellationToken cancellationToken = default);
    Task<RefreshToken?> FindRefreshToken(Expression<Func<RefreshToken, bool>> predicate, CancellationToken cancellationToken = default);
}
