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

namespace BookingService.API.Consumers;

public class CreateBookingsConsumeService(
	IRabbitMQConsumer<BookingModel> rabbitMQConsuner,
	IServiceScopeFactory serviceScopeFactory,
	ILogger<CreateBookingsConsumeService> logger,
	IBackgroundJobClient backgroundJobClient) : BackgroundService
{
	private readonly IRabbitMQConsumer<BookingModel> _rabbitMQConsuner = rabbitMQConsuner;
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
	private readonly ILogger<CreateBookingsConsumeService> _logger = logger;
	private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;

	protected override Task ExecuteAsync(CancellationToken cancellationToken)
	{
		_rabbitMQConsuner.ConsumeAsync(async (sender, args) =>
		{
			var booking = JsonSerializer.Deserialize<BookingModel>(
				Encoding.UTF8.GetString(args.Body.ToArray()));

			using var scope = _serviceScopeFactory.CreateScope();
			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

			await mediator.Send(new UpdateSeatsCommand(
				booking!.SessionId,
				booking!.Seats,
				true));

			await mediator.Send(new SaveBookingCommand(booking!));

			_backgroundJobClient.Schedule<CancelBookingAfterExpired>(
				b => b.ExecuteAsync(booking.Id, cancellationToken),
				JobsConstants.AFTER_BOOKING_EXPIRED_TEST);
		});

		return Task.CompletedTask;
	}
}