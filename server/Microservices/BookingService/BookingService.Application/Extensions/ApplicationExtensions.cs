using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
		{
			//cfg.RegisterServicesFromAssemblyContaining<CreateMovieCommandHandler>();
		});

		return services;
	}
}