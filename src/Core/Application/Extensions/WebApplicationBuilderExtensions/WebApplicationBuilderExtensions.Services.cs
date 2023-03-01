namespace Identity.Auth.Core.Application.Extensions.WebApplicationBuilderExtensions;

using Identity.Auth.Core.Application.MappingConfigs;
using Identity.Auth.Core.Application.Services;
using Identity.Auth.Core.Domain.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IJwtService, JwtService>();
		builder.Services.AddScoped<IUserService, UserService>();
		builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
		RegisterMapping();
		return builder;
	}

	private static void RegisterMapping()
	{
		MappingConfig.RegisterMappings();
	}
}
