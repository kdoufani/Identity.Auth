using BuildingBlocks.Core.Logging.File.Extensions.WebApplicationExtensions;
using BuildingBlocks.Core.Startup.Configurations;
using BuildingBlocks.Core.Startup.Extensions.WebApplicationBuilderExtensions;
using BuildingBlocks.Core.Startup.Extensions.WebApplicationExtensions;
using Identity.Auth.Core.Application.Extensions.Jwt;
using Identity.Auth.Core.Application.Extensions.WebApplicationBuilderExtensions;
using Identity.Auth.Core.Application.Security.Jwt;
using Identity.Auth.Core.Domain.Constants;
using Identity.Auth.Infrastructure.Data.Extensions.WebApplicationBuilderExtensions;
using Identity.Auth.Infrastructure.Data.Extensions.WebApplicationExtensions;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = BasicConfiguration.BasicConfigurationBuilder().Build();

builder.Services.AddSingleton(configuration);

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddApiVersioning();

builder.AddInfrastructure();

builder.AddCustomIdentity();

builder.AddServices();

builder.Services.AddCustomJwtAuthentication(configuration);

builder.Services.AddCustomAuthorization(
    rolePolicies: new List<RolePolicy>
    {
        new(IdentityRoleConstants.Admin, new List<string> {IdentityRoleConstants.Admin}),
        new(IdentityRoleConstants.User, new List<string> {IdentityRoleConstants.User})
    });

builder.Services.AddControllers();

builder.AddCustomSwagger(configuration);

builder.AddExceptionFilters();

builder.Services.AddMvc()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

builder.AddApilog();

var app = builder.Build();

var env = app.Environment;

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

if (!env.IsDevelopment())
{
    // the default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseApilogRequestLogging();

app.UseCustomSwagger(configuration);

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

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
