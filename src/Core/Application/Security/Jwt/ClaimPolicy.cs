namespace Identity.Auth.Core.Application.Security.Jwt;

using System.Security.Claims;

public class ClaimPolicy
{
    public ClaimPolicy(string name, IReadOnlyList<Claim>? claims)
    {
        Name = name;
        Claims = claims ?? new List<Claim>();
    }

    public string Name { get; set; }
    public IReadOnlyList<Claim> Claims { get; set; }
}
