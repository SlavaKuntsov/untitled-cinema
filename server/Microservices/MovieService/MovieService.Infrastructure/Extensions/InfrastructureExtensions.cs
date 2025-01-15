using Microsoft.Extensions.DependencyInjection;

using MovieService.Application.Interfaces.Caching;
using MovieService.Domain.Interfaces.Grpc;
using MovieService.Infrastructure.Caching;
using MovieService.Infrastructure.Grpc;

namespace MovieService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		services.AddScoped<IAuthGrpcService, AuthGrpcService>();
		services.AddScoped<IRedisCacheService, RedisCacheService>();

		return services;
	}
}