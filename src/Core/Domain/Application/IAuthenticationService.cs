namespace Identity.Auth.Core.Domain.Application;

using Identity.Auth.Core.Domain.Dtos;

public interface IAuthenticationService
{
    Task<LoginResponseDto> LoginAsync(string userNameOrEmail, string password);
}
