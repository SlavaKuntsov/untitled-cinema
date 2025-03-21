﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MovieService.Application.Interfaces.Caching;
using MovieService.Domain.Interfaces.Grpc;
using MovieService.Infrastructure.Caching;
using MovieService.Infrastructure.Grpc;

using StackExchange.Redis;

namespace MovieService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = Environment.GetEnvironmentVariable("REDIS_CONFIGURATION");

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration.GetConnectionString("Redis");

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
		services.AddScoped<IAuthGrpcService, AuthGrpcService>();

		return services;
	}
}