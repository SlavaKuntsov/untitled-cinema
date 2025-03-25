using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Redis;

public static class RedisExtension
{
	public static IServiceCollection AddRedis(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var connectionString = Environment.GetEnvironmentVariable("REDIS_CONFIGURATION");

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration.GetConnectionString("Redis");

		services.AddStackExchangeRedisCache(options =>
		{
			options.Configuration = connectionString;
			options.InstanceName = string.Empty;
		});

		services.AddSingleton<IConnectionMultiplexer>(_ =>
		{
			var configuration = ConfigurationOptions.Parse(connectionString, true);

			return ConnectionMultiplexer.Connect(configuration);
		});

		return services;
	}
}