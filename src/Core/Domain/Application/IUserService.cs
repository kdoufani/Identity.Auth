namespace Identity.Auth.Core.Domain.Application;

using Identity.Auth.Core.Domain.Enums;
using Identity.Auth.Core.Domain.Dtos;

public interface IUserService
{
    Task<IdentityUserDto> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IdentityUserDto> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IdentityUserDto> RegisterUserAsync(RegisterUserDto userDto, CancellationToken cancellationToken = default);
    Task UpdateUserStateAsync(string userId, UserState userState);
}
