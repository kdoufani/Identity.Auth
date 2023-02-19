namespace Identity.Auth.Core.Application.Extensions.Jwt;

using Identity.Auth.Core.Application.Security.Jwt;
using Identity.Auth.Core.Domain.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

public static class JwtExtensions
{
    public static AuthenticationBuilder AddCustomJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

        // https://docs.microsoft.com/en-us/aspnet/core/security/authentication
        // https://learn.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-6.0#use-multiple-authentication-schemes
        // https://auth0.com/blog/whats-new-in-dotnet-7-for-authentication-and-authorization/
        // since .NET 7, the default scheme is no longer required, when we define just one authentication scheme and It is automatically inferred
        return services.AddAuthentication() // no default scheme specified
            .AddJwtBearer(options =>
            {
                //-- JwtBearerDefaults.AuthenticationScheme --
                options.Audience = jwtOptions.Audience;
                options.SaveToken = true;
                options.RefreshOnIssuerKeyNotFound = false;
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    SaveSigninToken = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            throw new UnAuthorizedException("The Token is expired.");
                        }

                        throw new IdentityException(
                            context.Exception.Message,
                            statusCode: HttpStatusCode.InternalServerError);
                    },
                    OnChallenge = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnForbidden = _ =>
                        throw new ForbiddenException("You are not authorized to access this resource.")
                };
            });
    }


    public static IServiceCollection AddCustomAuthorization(
        this IServiceCollection services,
        IList<ClaimPolicy>? claimPolicies = null,
        IList<RolePolicy>? rolePolicies = null)
    {
        services.AddAuthorization(authorizationOptions =>
        {
            // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme
            // https://andrewlock.net/setting-global-authorization-policies-using-the-defaultpolicy-and-the-fallbackpolicy-in-aspnet-core-3/
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme);
            defaultAuthorizationPolicyBuilder =
                defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
            authorizationOptions.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

            // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/claims
            if (claimPolicies is { })
            {
                foreach (var policy in claimPolicies)
                {
                    authorizationOptions.AddPolicy(policy.Name, x =>
                    {
                        x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        foreach (var policyClaim in policy.Claims)
                        {
                            x.RequireClaim(policyClaim.Type, policyClaim.Value);
                        }
                    });
                }
            }

            // https://docs.microsoft.com/en-us/aspnet/core/security/authorization
            if (rolePolicies is { })
            {
                foreach (var rolePolicy in rolePolicies)
                {
                    authorizationOptions.AddPolicy(rolePolicy.Name, x =>
                    {
                        x.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        x.RequireRole(rolePolicy.Roles);
                    });
                }
            }
        });

        return services;
    }
}
