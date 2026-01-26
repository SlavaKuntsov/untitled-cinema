using BookingService.Application.Consumers;
using BookingService.Application.Handlers.Commands.Bookings.CreateBooking;
using BookingService.Application.Jobs.Bookings;
using Brokers.Models.Request;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddHostedService<CreateBookingsConsumeService>();
		services.AddHostedService<CreateSeatsConsumeService>();

		services.AddMediatR(
			cfg =>
			{
				cfg.RegisterServicesFromAssemblyContaining<CreateBookingCommandHandler>();
			});

		return services;
	}
}