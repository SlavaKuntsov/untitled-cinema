using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Redis;
using Redis.Services;
using UserService.Application.Interfaces.Auth;
using UserService.Application.Interfaces.Notification;
using UserService.Infrastructure.Auth;
using UserService.Infrastructure.Hubs.Notification;
using Utilities.Service;

namespace UserService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSignalR();

		services.AddRedis(configuration);

		services.AddScoped<ICookieService, CookieService>();
		services.AddScoped<IPasswordHash, PasswordHash>();
		services.AddScoped<IJwt, Jwt>();
		services.AddScoped<INotificationService, NotificationService>();

		//var emailOptions = configuration.GetSection(nameof(EmailModel));

		//services.AddScoped<IEmailService>();

		return services;
	}
}