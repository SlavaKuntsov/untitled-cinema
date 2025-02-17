using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

using UserService.Application.Interfaces.Auth;
using UserService.Application.Interfaces.Caching;
using UserService.Application.Interfaces.Notification;
using UserService.Infrastructure.Auth;
using UserService.Infrastructure.Caching;
using UserService.Infrastructure.Notification;

namespace UserService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSignalR();

		var connectionString = Environment.GetEnvironmentVariable("REDIS_CONFIGURATION");

		services.AddStackExchangeRedisCache(options =>
		{
			options.Configuration = connectionString;
			options.InstanceName = string.Empty;
		});

		services.AddSingleton<IConnectionMultiplexer>(sp =>
		{
			var configuration =
				ConfigurationOptions.Parse(
					connectionString, true);
			return ConnectionMultiplexer.Connect(configuration);
		});

		services.AddScoped<IRedisCacheService, RedisCacheService>();
		services.AddScoped<ICookieService, CookieService>();
		services.AddScoped<IPasswordHash, PasswordHash>();
		services.AddScoped<IJwt, Jwt>();
		services.AddScoped<INotificationService, NotificationService>();

		//var emailOptions = configuration.GetSection(nameof(EmailModel));

		//services.AddScoped<IEmailService>();

		return services;
	}
}