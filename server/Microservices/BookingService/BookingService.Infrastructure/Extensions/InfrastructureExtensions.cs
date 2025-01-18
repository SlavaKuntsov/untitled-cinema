using BookingService.Domain.Interfaces.Grpc;
using BookingService.Infrastructure.Grpc;

using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		services.AddScoped<IAuthGrpcService, AuthGrpcService>();

		return services;
	}
}