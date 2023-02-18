using BuildingBlocks.Core.Startup.Extensions;
using Identity.Auth.Api.Core;
using Identity.Auth.Infrastructure.Data.Extensions.WebApplicationExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices();

builder.AddCrmlog();

var app = builder.Build();

var environment = app.Environment;

app.Configure(environment);

await app.SeedData();

app.Run();
