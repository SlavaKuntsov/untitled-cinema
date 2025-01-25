using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;

using MapsterMapper;

using MediatR;

using MovieService.Application.Handlers.Queries.Movies.GetMovieById;
using MovieService.Application.Handlers.Queries.Seats.GetAllSeatById;
using MovieService.Application.Handlers.Queries.Seats.GetSeatTypeById;
using MovieService.Application.Handlers.Queries.Sessions.GetSessionById;
using MovieService.Domain.Models;

namespace MovieService.API.Consumers.SessionAndSeats;

public class SessionAndSeatsConsumeService(
	IRabbitMQConsumer<SessionAndSeatsResponse> rabbitMQConsuner,
	IServiceScopeFactory serviceScopeFactory,
	IMapper mapper) : BackgroundService
{
	private readonly IRabbitMQConsumer<SessionAndSeatsResponse> _rabbitMQConsuner = rabbitMQConsuner;
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
	private readonly IMapper _mapper = mapper;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await _rabbitMQConsuner
			.RequestReplyAsync<SessionAndSeatsRequest>(async (request) =>
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var session = await _mediator.Send(new GetSessionByIdQuery(request.SessionId));

				if (session is null)
					return new SessionAndSeatsResponse($"Session with id '{request.SessionId}' not found.");

				var seats = await _mediator.Send(new GetSeatsBySessionIdQuery(request.SessionId));

				var seatsRequest = _mapper.Map<IList<SeatModel>>(request.Seats);

				var missingIds = seatsRequest
					.Where(reqSeat => !seats.Any(seat =>
						seat.Id == reqSeat.Id &&
						seat.Row == reqSeat.Row &&
						seat.Column == reqSeat.Column))
					.Select(reqSeat => reqSeat.Id)
					.ToList();

				if (missingIds.Any())
					return new SessionAndSeatsResponse(
						$"Seat(-s) with id's '{string.Join(", ", missingIds)}' not found.");

				var movie = await _mediator.Send(new GetMovieByIdQuery(session.MovieId));

				if (movie is null)
					return new SessionAndSeatsResponse($"Movie with id '{session.MovieId.ToString()}' not found.");

				var price = 0m;

				foreach (var item in seats)
				{
					var seatType = await _mediator.Send(new GetSeatTypeByIdQuery(item.SeatTypeId));

					price += seatType.PriceModifier * session.PriceModifier * movie.Price;
				}

				return new SessionAndSeatsResponse("", price);
			}, stoppingToken);

		return;
	}
}