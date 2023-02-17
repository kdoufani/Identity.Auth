namespace Identity.Auth.Core.Domain.Dtos;

public class RegisterUserDto
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public string ConfirmPassword { get; init; }
    public IEnumerable<string>? Roles { get; init; }
}
