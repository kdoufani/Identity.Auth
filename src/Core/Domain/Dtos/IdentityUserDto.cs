namespace Identity.Auth.Core.Domain.Dtos;

using Identity.Auth.Core.Domain.Enums;

public class IdentityUserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime? LastLoggedInAt { get; set; }
    public IEnumerable<string>? RefreshTokens { get; set; }
    public IEnumerable<string>? Roles { get; set; }
    public UserState UserState { get; set; }
    public DateTime CreatedAt { get; set; }
}
