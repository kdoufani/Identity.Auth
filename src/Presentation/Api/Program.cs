using BuildingBlocks.Core.Logging.File;
using BuildingBlocks.Core.Securities;
using BuildingBlocks.Core.Startup.Configurations;
using BuildingBlocks.Core.Startup.Extensions;
using Identity.Auth.Api.Configurations;
using Identity.Auth.Core.Application.Extensions.WebApplicationBuilderExtensions;
using Identity.Auth.Infrastructure.Data.Extensions.WebApplicationBuilderExtensions;
using Identity.Auth.Infrastructure.Data.Extensions.WebApplicationExtensions;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = BasicConfiguration.BasicConfigurationBuilder().Build();
var configurationOptions = configuration.Get<ConfigurationOptions>();

builder.Services.AddSingleton(configuration);

builder.Services.AddScoped<OperationIdentifiers>();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));


builder.Services.AddApiVersioning();

builder.Services.AddControllers();

builder.AddInfrastructure();

builder.AddCustomIdentity();

builder.AddServices();

builder.Services.ConfigureSwaggerService(configurationOptions);

builder.Services.AddExceptionFilters();

builder.Services.AddMvc()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

builder.AddCrmlog();

var app = builder.Build();

var env = app.Environment;

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseHeadersValidation();

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

await app.ApplyDatabaseMigrations();
await app.SeedData();

app.Run();
