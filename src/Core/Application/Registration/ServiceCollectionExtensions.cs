namespace Identity.Auth.Core.Application.Registration;

using Identity.Auth.Core.Application.Services;
using Identity.Auth.Core.Domain.Application;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterServices(this IServiceCollection services)
	{
		services.AddScoped<IUserService, UserService>();
		return services;
	}
}
