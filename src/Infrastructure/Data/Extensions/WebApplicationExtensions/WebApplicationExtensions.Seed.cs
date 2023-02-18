namespace Identity.Auth.Infrastructure.Data.Extensions.WebApplicationExtensions;

using Identity.Auth.Infrastructure.Data.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static partial class WebApplicationExtensions
{
    public static async Task SeedData(this WebApplication app)
    {
        // https://stackoverflow.com/questions/38238043/how-and-where-to-call-database-ensurecreated-and-database-migrate
        // https://www.michalbialecki.com/2020/07/20/adding-entity-framework-core-5-migrations-to-net-5-project/
        using var serviceScope = app.Services.CreateScope();
        var seeders = serviceScope.ServiceProvider.GetServices<IDataSeeder>();

        foreach (var seeder in seeders)
        {
            app.Logger.LogInformation("Seeding '{Seed}' started...", seeder.GetType().Name);
            await seeder.SeedAllAsync();
            app.Logger.LogInformation("Seeding '{Seed}' ended...", seeder.GetType().Name);
        }
    }
}
