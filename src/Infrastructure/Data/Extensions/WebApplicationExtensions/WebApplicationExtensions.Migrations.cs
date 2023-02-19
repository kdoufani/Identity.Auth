namespace Identity.Auth.Infrastructure.Data.Extensions.WebApplicationExtensions;

using Identity.Auth.Core.Infrastructure.Data.DbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static partial class WebApplicationExtensions
{
    public static async Task ApplyDatabaseMigrations(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<IdentityContext>();

        app.Logger.LogInformation("Updating database...");

        await dbContext.Database.MigrateAsync();

        app.Logger.LogInformation("Updated database");
    }
}
