using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis;

namespace MovieService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddRedis(configuration);

		// services.AddScoped<IAuthGrpcService, AuthGrpcService>();
		// services.AddScoped<ISeatsGrpcService, SeatsGrpcService>();

		return services;
	}
}