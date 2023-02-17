namespace Identity.Auth.Core.Application.Security.Jwt;

using Identity.Auth.Core.Domain.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class JwtHandler
{
    public static JwtTokenDto GenerateJwtToken(
        string userName,
        string email,
        string userId,
        JwtOptions jwtOptions,
        string? fullName = null,
        string? refreshToken = null,
        IReadOnlyList<Claim>? usersClaims = null,
        IReadOnlyList<string>? rolesClaims = null,
        IReadOnlyList<string>? permissionsClaims = null)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User ID claim (subject) cannot be empty.", nameof(userName));

        var now = DateTime.Now;

        // https://leastprivilege.com/2017/11/15/missing-claims-in-the-asp-net-core-2-openid-connect-handler/
        // https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/a301921ff5904b2fe084c38e41c969f4b2166bcb/src/System.IdentityModel.Tokens.Jwt/ClaimTypeMapping.cs#L45-L125
        // https://stackoverflow.com/a/50012477/581476
        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.NameId, userId),
            new(JwtRegisteredClaimNames.Name, fullName ?? ""),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Sid, userId),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.GivenName, fullName ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)),
            new(CustomClaimTypes.RefreshToken, refreshToken ?? ""),
        };

        if (rolesClaims?.Any() is true)
        {
            foreach (var role in rolesClaims)
                jwtClaims.Add(new Claim(ClaimTypes.Role, role.ToLower(CultureInfo.InvariantCulture)));
        }

        if (!string.IsNullOrWhiteSpace(jwtOptions.Audience))
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, jwtOptions.Audience));

        if (permissionsClaims?.Any() is true)
        {
            foreach (var permissionsClaim in permissionsClaims)
            {
                jwtClaims.Add(new Claim(
                    CustomClaimTypes.Permission,
                    permissionsClaim.ToLower(CultureInfo.InvariantCulture)));
            }
        }

        if (usersClaims?.Any() is true)
            jwtClaims = jwtClaims.Union(usersClaims).ToList();


        SymmetricSecurityKey signingKey = new (Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
        SigningCredentials signingCredentials = new (signingKey, SecurityAlgorithms.HmacSha256);

        var expireTime = now.AddSeconds(jwtOptions.TokenLifeTimeSecond == 0 ? 300 : jwtOptions.TokenLifeTimeSecond);
        var jwt = new JwtSecurityToken(
            jwtOptions.Issuer,
            jwtOptions.Audience,
            notBefore: now,
            claims: jwtClaims,
            expires: expireTime,
            signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new JwtTokenDto() { Token = token, ExpireAt = expireTime };
    }

    public static ClaimsPrincipal? GetPrincipalFromToken(string token, JwtOptions jwtOptions)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
        };

        JwtSecurityTokenHandler tokenHandler = new();

        ClaimsPrincipal principal = tokenHandler.ValidateToken(
            token,
            tokenValidationParameters,
            out SecurityToken securityToken);


        if (securityToken is not JwtSecurityToken)
        {
            throw new SecurityTokenException("Invalid access token.");
        }

        return principal;
    }
}
