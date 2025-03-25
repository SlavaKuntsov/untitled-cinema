using System.Text;
using System.Text.Json;
using BookingService.Application.Handlers.Commands.Bookings.SaveBooking;
using BookingService.Application.Handlers.Commands.Seats.UpdateSeats;
using BookingService.Application.Jobs.Bookings;
using BookingService.Domain.Constants;
using BookingService.Domain.Models;
using Brokers.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Consumers;

public class CreateBookingsConsumeService(
	IRabbitMQConsumer<BookingModel> rabbitMQConsumer,
	IServiceScopeFactory serviceScopeFactory,
	IBackgroundJobClient backgroundJobClient,
	ILogger<CreateBookingsConsumeService> logger) : BackgroundService
{
	protected override Task ExecuteAsync(CancellationToken cancellationToken)
	{
		rabbitMQConsumer.ConsumeAsync(
			async (_, args) =>
			{
				var booking = JsonSerializer.Deserialize<BookingModel>(
					Encoding.UTF8.GetString(args.Body.ToArray()));

				using var scope = serviceScopeFactory.CreateScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				await mediator.Send(
					new UpdateSeatsCommand(
						booking!.SessionId,
						booking!.Seats,
						true),
					cancellationToken);

				await mediator.Send(
					new SaveBookingCommand(booking),
					cancellationToken);

				backgroundJobClient.Schedule<CancelBookingAfterExpired>(
					b => b.ExecuteAsync(booking.Id, cancellationToken),
					JobsConstants.AFTER_BOOKING_EXPIRED_TEST);
			});

		return Task.CompletedTask;
	}
}