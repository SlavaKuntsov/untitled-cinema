using BookingService.Application.Handlers.Commands.Bookings.CreateBooking;

using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblyContaining<CreateBookingCommandHandler>();
		});

		return services;
	}
}