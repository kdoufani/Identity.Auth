namespace Identity.Auth.Infrastructure.Data.Extensions.WebApplicationBuilderExtensions;

using Identity.Auth.Core.Domain.Repositories;
using Identity.Auth.Infrastructure.Data.Persistence;
using Identity.Auth.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IJwtRepository, JwtRepository>();

        builder.Services.AddScoped<IDataSeeder, IdentityDataSeeder>();

        return builder;
    }
}
