namespace Identity.Auth.Core.Domain.Entities;

public class AccessToken
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiredAt { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
}
