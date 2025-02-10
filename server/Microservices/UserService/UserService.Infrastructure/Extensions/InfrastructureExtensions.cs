using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using UserService.Application.Interfaces.Auth;
using UserService.Application.Interfaces.Email;
using UserService.Infrastructure.Auth;
using UserService.Infrastructure.Email;

namespace UserService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<ICookieService, CookieService>();
		services.AddScoped<IPasswordHash, PasswordHash>();
		services.AddScoped<IJwt, Jwt>();

		//var emailOptions = configuration.GetSection(nameof(EmailModel));

		//services.AddScoped<IEmailService>();

		return services;
	}
}