namespace Identity.Auth.Infrastructure.Data.Extensions.WebApplicationBuilderExtensions;

using Identity.Auth.Core.Domain.Entities;
using Identity.Auth.Core.Infrastructure.Data.DbContext;
using Identity.Auth.Data.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCustomIdentity(this WebApplicationBuilder builder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var config = builder.Configuration.GetSection(nameof(PostgresOptions)).Get<PostgresOptions>();

        builder.Services.AddDbContext<IdentityContext>(options =>
        {
            options.UseNpgsql(config.DefaultConnection, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly((typeof(IdentityContext).Assembly).GetName().Name);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });

        // Problem with .net core identity - will override our default authentication scheme `JwtBearerDefaults.AuthenticationScheme` to unwanted `ECommerce.Services.Identity.Application` in `AddIdentity()` method .net identity
        // https://github.com/IdentityServer/IdentityServer4/issues/1525
        // https://github.com/IdentityServer/IdentityServer4/issues/1525
        // some dependencies will add here if not registered before
        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 3;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.RequireUniqueEmail = true;

        })
        .AddEntityFrameworkStores<IdentityContext>()
        .AddDefaultTokenProviders();

        return builder;
    }
}
