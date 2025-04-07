using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieService.Domain.Interfaces.Grpc;
using MovieService.Infrastructure.Grpc;
using Redis;
using Redis.Services;

namespace MovieService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddRedis(configuration);

		services.AddScoped<IRedisCacheService, RedisCacheService>();
		services.AddScoped<IAuthGrpcService, AuthGrpcService>();
		services.AddScoped<ISeatsGrpcService, SeatsGrpcService>();

		return services;
	}
}