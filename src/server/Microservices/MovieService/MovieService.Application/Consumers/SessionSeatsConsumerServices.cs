using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieService.Application.Consumers;
using MovieService.Application.Handlers.Queries.Seats.GetSeatsBySessionId;
using MovieService.Application.Handlers.Queries.Sessions.GetSessionById;
using MovieService.Domain.Models;

namespace MovieService.API.Consumers;

public class SessionSeatsConsumerServices(
	IRabbitMQConsumer<SessionSeatsResponse> rabbitMqConsumer,
	IServiceScopeFactory serviceScopeFactory,
	ILogger<BookingPriceConsumeService> logger,
	IMapper mapper) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await rabbitMqConsumer
			.RequestReplyAsync<SessionSeatsRequest>(
				async request =>
				{
					using var scope = serviceScopeFactory.CreateScope();
					var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

					logger.LogInformation("Starting to consume session seats");

					var session = await mediator.Send(
						new GetSessionByIdQuery(request.SessionId),
						stoppingToken);

					if (session is null)
						return new SessionSeatsResponse(
							$"Session with id '{request.SessionId}' not found.");

					var seatModels = await mediator.Send(
						new GetSeatsBySessionIdQuery(request.SessionId),
						stoppingToken);

					if (!seatModels.Any())
						return new SessionSeatsResponse(
							$"Hall with id '{request.SessionId}' doesn't have any seats.");

					var seats = mapper.Map<IList<BookingService.Domain.Models.SeatModel>>(seatModels);

					return new SessionSeatsResponse("", seats);
				},
				stoppingToken);
	}
}