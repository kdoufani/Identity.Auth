namespace Identity.Auth.Data.Registration.Configuration;

using BuildingBlocks.Core.Startup.Configurations.Options;

public class ConfigurationOptions : SharedConfigurationOptions
{
    public PostgresOptions Database { get; set; }
}
