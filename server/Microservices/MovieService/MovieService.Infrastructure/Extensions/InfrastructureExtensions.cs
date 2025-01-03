using Microsoft.Extensions.DependencyInjection;

using MovieService.Domain.Interfaces.Grpc;
using MovieService.Infrastructure.Grpc;

namespace MovieService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		services.AddScoped<IAuthGrpcService, AuthGrpcService>();

		return services;
	}
}