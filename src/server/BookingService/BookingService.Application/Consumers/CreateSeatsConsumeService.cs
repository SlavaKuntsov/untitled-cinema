using BookingService.Application.Handlers.Commands.Seats.CreateEmptySeats;
using BookingService.Domain.Models;
using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookingService.Application.Consumers;

public class CreateSeatsConsumeService(
	IRabbitMQConsumer<CreateSeatsResponse> rabbitMqConsumer,
	IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await rabbitMqConsumer
			.RequestReplyAsync<CreateSeatsRequest>(async request =>
				{
					using var scope = serviceScopeFactory.CreateScope();
					var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

					var seats = new List<SeatModel>(request.Seats.Count);

					foreach (var seat in request.Seats)
						seats.Add(new SeatModel(seat.Id, seat.Row, seat.Column));

					Console.WriteLine(seats);

					await mediator.Send(
						new CreateEmptySeats(request.SessionId, seats),
						stoppingToken);

					Console.WriteLine("Created empty seats.");

					return new CreateSeatsResponse("");
				},
				stoppingToken);
	}
}