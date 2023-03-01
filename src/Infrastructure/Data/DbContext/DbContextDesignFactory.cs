namespace Identity.Auth.Core.Infrastructure.Data.DbContext;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DbContextDesignFactory : IDesignTimeDbContextFactory<IdentityContext>
{
    private const string _connectionStringSection = "PostgresOptions:DefaultConnection";

    public IdentityContext CreateDbContext(string[] args)
    {
        Console.WriteLine($"BaseDirectory: {AppContext.BaseDirectory}");

        //IConfiguration configuration = BasicConfiguration.BasicConfigurationBuilder().Build();
        var connectionStringSectionValue = "Host=localhost;Port=5432;Database=Identity_Auth;Username=postgres;Password=mypassword";

        if (string.IsNullOrWhiteSpace(connectionStringSectionValue))
        {
            throw new InvalidOperationException($"Could not find a value for {_connectionStringSection} section.");
        }

        Console.WriteLine($"ConnectionString  section value is : {connectionStringSectionValue}");

        var optionsBuilder = new DbContextOptionsBuilder<IdentityContext>()
            .UseNpgsql(
                connectionStringSectionValue,
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(GetType().Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                }
            );

        return (IdentityContext)Activator.CreateInstance(typeof(IdentityContext), optionsBuilder.Options);
    }
}


