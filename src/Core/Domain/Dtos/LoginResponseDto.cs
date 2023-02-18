namespace Identity.Auth.Core.Domain.Dtos;

using Identity.Auth.Core.Domain.Entities;

public class LoginResponseDto
{
    public Guid UserId { get; init; }
    public string AccessToken { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Username { get; init; }
    public string RefreshToken { get; init; }
}
