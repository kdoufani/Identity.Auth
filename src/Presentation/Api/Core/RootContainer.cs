namespace Identity.Auth.Api.Core;

using BuildingBlocks.Core.Logging.File;
using BuildingBlocks.Core.Startup.Configurations;
using BuildingBlocks.Core.Startup.Extensions;
using Identity.Auth.Core.Application.Registration;
using Identity.Auth.Core.Infrastructure.Data.Registration;
using Identity.Auth.Data.Registration.Configuration;
using Newtonsoft.Json.Converters;

internal static class RootContainer
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        IConfiguration configuration = BasicConfiguration.BasicConfigurationBuilder().Build();
        var configurationOptions = configuration.Get<ConfigurationOptions>();

        services.AddSingleton(configuration);

        services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }));


        services.AddApiVersioning();

        services.AddControllers();

        services
            .RegisterDatabase(configuration)
            .RegisterServices();

        services.ConfigureSwaggerService(configurationOptions);

        services.AddExceptionFilters();

        services.AddMvc()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

        return services;
    }

    public static IApplicationBuilder Configure(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        if (!env.IsDevelopment())
        {
            // the default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseCrmlogRequestLogging();

        app.ConfigureSwagger();

        app.UseRouting();

        app.UseEndpoints(endPoints =>
        {
            endPoints.MapGet("/ping", context =>
            {
                context.Response.StatusCode = 200;
                return Task.CompletedTask;
            });
            endPoints.MapControllers();
        });

        return app;
    }
}
