using BookingService.Domain.Models;

using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;

using MapsterMapper;

using MediatR;

using MovieService.Application.Handlers.Queries.Seats.GetAllSeatById;
using MovieService.Application.Handlers.Queries.Sessions.GetSessionById;

namespace MovieService.API.Consumers;

public class SessionSeatsConsumerServices(
	IRabbitMQConsumer<SessionSeatsResponse> rabbitMQConsuner,
	IServiceScopeFactory serviceScopeFactory,
	IMapper mapper) : BackgroundService
{
	private readonly IRabbitMQConsumer<SessionSeatsResponse> _rabbitMQConsuner = rabbitMQConsuner;
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
	private readonly IMapper _mapper = mapper;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await _rabbitMQConsuner
			.RequestReplyAsync<SessionSeatsRequest>(async (request) =>
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var session = await mediator.Send(new GetSessionByIdQuery(request.SessionId));

				if (session is null)
					return new SessionSeatsResponse($"Session with id '{request.SessionId}' not found.");

				var seatModels = await mediator.Send(new GetSeatsBySessionIdQuery(request.SessionId));

				if(!seatModels.Any())
					return new SessionSeatsResponse($"Hall with id '{request.SessionId}' doesn't have any seats.");

				var seats = _mapper.Map<IList<SeatModel>>(seatModels);

				return new SessionSeatsResponse("", seats);
			},
			stoppingToken);

		return;
	}
}
