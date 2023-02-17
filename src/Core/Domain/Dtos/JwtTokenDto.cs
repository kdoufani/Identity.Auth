namespace Identity.Auth.Core.Domain.Dtos;

public class JwtTokenDto
{
    public string Token { get; set; }
    public DateTime ExpireAt { get; set; }
}
