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
	IRabbitMQConsumer<SessionSeatsResponse<SeatModel>> rabbitMqConsumer,
	IServiceScopeFactory serviceScopeFactory,
	ILogger<BookingPriceConsumeService> logger,
	IMapper mapper) : BackgroundService
{
	private readonly ILogger<BookingPriceConsumeService> _logger = logger;

	private readonly IMapper _mapper = mapper;

	private readonly IRabbitMQConsumer<SessionSeatsResponse<SeatModel>> _rabbitMQConsumer = rabbitMqConsumer;

	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await _rabbitMQConsumer
			.RequestReplyAsync<SessionSeatsRequest>(async request =>
				{
					using var scope = _serviceScopeFactory.CreateScope();
					var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

					_logger.LogInformation("Starting to consume session seats");

					var session = await mediator.Send(
						new GetSessionByIdQuery(request.SessionId),
						stoppingToken);

					if (session is null)
						return new SessionSeatsResponse<SeatModel>($"Session with id '{request.SessionId}' not found.");

					var seatModels = await mediator.Send(
						new GetSeatsBySessionIdQuery(request.SessionId),
						stoppingToken);

					if (!seatModels.Any())
						return new SessionSeatsResponse<SeatModel>(
							$"Hall with id '{request.SessionId}' doesn't have any seats.");

					var seats = _mapper.Map<IList<SeatModel>>(seatModels);

					return new SessionSeatsResponse<SeatModel>("", seats);
				},
				stoppingToken);
	}
}