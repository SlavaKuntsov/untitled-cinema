using Microsoft.Extensions.DependencyInjection;

using UserService.Application.Interfaces.Auth;
using UserService.Infrastructure.Auth;

namespace UserService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		services.AddScoped<ICookieService, CookieService>();
		services.AddScoped<IPasswordHash, PasswordHash>();
		services.AddScoped<IJwt, Jwt>();

		return services;
	}
}