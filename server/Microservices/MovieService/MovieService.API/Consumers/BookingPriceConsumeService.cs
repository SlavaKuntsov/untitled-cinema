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

namespace MovieService.API.Consumers;

public class BookingPriceConsumeService(
	IRabbitMQConsumer<BookingPriceResponse> rabbitMQConsuner,
	IServiceScopeFactory serviceScopeFactory,
	IMapper mapper) : BackgroundService
{
	private readonly IRabbitMQConsumer<BookingPriceResponse> _rabbitMQConsuner = rabbitMQConsuner;
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
	private readonly IMapper _mapper = mapper;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await _rabbitMQConsuner
			.RequestReplyAsync<BookingPriceRequest>(async (request) =>
			{
				using var scope = _serviceScopeFactory.CreateScope();
				var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				var session = await mediator.Send(new GetSessionByIdQuery(request.SessionId));

				if (session is null)
					return new BookingPriceResponse($"Session with id '{request.SessionId}' not found.");

				var seats = await mediator.Send(new GetSeatsBySessionIdQuery(request.SessionId));

				var seatsRequest = _mapper.Map<IList<SeatModel>>(request.Seats);

				var missingIds = seatsRequest
					.Where(reqSeat => !seats.Any(seat =>
						seat.Id == reqSeat.Id &&
						seat.Row == reqSeat.Row &&
						seat.Column == reqSeat.Column))
					.Select(reqSeat => reqSeat.Id)
					.ToList();

				if (missingIds.Any())
				{
					return new BookingPriceResponse(
						$"Seat(-s) with id's '{string.Join(", ", missingIds)}' not found.");
				}

				var movie = await mediator.Send(new GetMovieByIdQuery(session.MovieId));

				if (movie is null)
					return new BookingPriceResponse($"Movie with id '{session.MovieId.ToString()}' not found.");

				var price = 0m;

				var selectedSeats = seats.Where(
					seat => seatsRequest.Any(
						reqSeat => reqSeat.Id == seat.Id));

				foreach (var item in selectedSeats)
				{
					var seatType = await mediator.Send(new GetSeatTypeByIdQuery(item.SeatTypeId));

					price += seatType.PriceModifier * session.PriceModifier * movie.Price;
				}

				return new BookingPriceResponse("", price);
			}, stoppingToken);

		return;
	}
}