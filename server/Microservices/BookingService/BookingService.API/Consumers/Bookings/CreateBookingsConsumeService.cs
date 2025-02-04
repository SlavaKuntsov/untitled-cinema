using System.Text;
using System.Text.Json;

using BookingService.Application.Handlers.Commands.Bookings.SaveBooking;
using BookingService.Application.Handlers.Commands.Seats.UpdateSeats;
using BookingService.Domain.Models;

using Brokers.Interfaces;

using MapsterMapper;

using MediatR;

namespace BookingService.API.Consumers.Bookings;

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

			//if (booking is null)
			//	return;

			using var scope = _serviceScopeFactory.CreateScope();
			var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

			await mediator.Send(new UpdateSeatsCommand(
				booking!.SessionId, 
				booking!.Seats,
				true));

			// if error(exception) maybe add error in the table with this booking status

			await mediator.Send(new SaveBookingCommand(booking!));

		});

		return Task.CompletedTask;
	}
}