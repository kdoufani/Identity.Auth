using BuildingBlocks.Core.Startup.Extensions;
using Identity.Auth.Api.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices();

builder.AddCrmlog();

var app = builder.Build();

var environment = app.Environment;

app.Configure(environment);

app.Run();
