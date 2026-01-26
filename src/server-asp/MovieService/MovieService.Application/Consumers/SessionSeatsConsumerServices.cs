using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;
using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieService.Application.Handlers.Queries.Seats.GetSeatsBySessionId;
using MovieService.Application.Handlers.Queries.Sessions.GetSessionById;
using MovieService.Domain.Models;
using SeatModel = BookingService.Domain.Models.SeatModel;

namespace MovieService.Application.Consumers;

public class SessionSeatsConsumerServices(
	IRabbitMQConsumer<SessionSeatsResponse> rabbitMqConsumer,
	IServiceScopeFactory serviceScopeFactory,
	ILogger<BookingPriceConsumeService> logger,
	IMapper mapper) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await rabbitMqConsumer.RequestReplyAsync<SessionSeatsRequest>(
			async request =>
			{
				using var scope = serviceScopeFactory.CreateScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				logger.LogInformation("Starting to consume session seats");

				SessionModel session;
				try
				{
					session = await mediator.Send(
						new GetSessionByIdQuery(request.SessionId),
						stoppingToken);
				}
				catch (NotFoundException e)
				{
					logger.LogWarning($"Session with id '{request.SessionId}' not found.");

					return new SessionSeatsResponse(
						$"Session with id '{request.SessionId}' not found.");
				}
				catch (Exception e)
				{
					Console.WriteLine(e);

					throw;
				}

				// if (session == null)

				var seatModels = await mediator.Send(
					new GetSeatsBySessionIdQuery(request.SessionId),
					stoppingToken);

				if (!seatModels.Any())
				{
					logger.LogWarning($"Hall with id '{session.HallId}' doesn't have any seats.");

					return new SessionSeatsResponse(
						$"Hall with id '{session.HallId}' doesn't have any seats.");
				}

				var seats = mapper.Map<IList<SeatModel>>(seatModels);

				logger.LogInformation(
					"Successfully retrieved {SeatCount} seats for session {SessionId}",
					seats.Count,
					request.SessionId);

				// if (session == null)
				// {
				// 	logger.LogWarning($"Session with id '{request.SessionId}' not found.");
				// 	return new SessionSeatsResponse(
				// 		$"Session with id '{request.SessionId}' not found.");
				// }

				return new SessionSeatsResponse("", seats);
			},
			stoppingToken);
	}
}