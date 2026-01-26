using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using MovieService.Application.DTOs;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatByIndex;

public record GetSeatByIndexQuery(
	Guid SessionId,
	int Row,
	int Column) : IRequest<SelectedSeatDto>;

public class GetSeatByIndexQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetSeatByIndexQuery, SelectedSeatDto>
{
	public async Task<SelectedSeatDto> Handle(
		GetSeatByIndexQuery request,
		CancellationToken cancellationToken)
	{
		var session = await unitOfWork.SessionsRepository.GetHallIdAsync(request.SessionId, cancellationToken)
			?? throw new NotFoundException($"Hall with sessionId {request.SessionId} not found.");
		
		var seat = await unitOfWork.SeatsRepository.GetAsync(
						session.HallId,
						request.Row,
						request.Column,
						cancellationToken)
					?? throw new NotFoundException(
						$"Seat with row '{request.Row}' " +
						$"and column '{request.Column}' not found.");

		var seatType = await unitOfWork.SeatsRepository.GetTypeAsync(
							seat.SeatTypeId,
							cancellationToken)
						?? throw new NotFoundException(
							$"Seat Type with id '{seat.SeatTypeId}' not found.");


		var movie = await unitOfWork.MoviesRepository.GetAsync(session.MovieId, cancellationToken)
			?? throw new NotFoundException($"Movie with id '{session.MovieId}' not found.");

		var price = seatType.PriceModifier * session.PriceModifier * movie.Price;
		
		var selectedSeat = mapper.Map<SelectedSeatDto>(seat);

		selectedSeat = selectedSeat with
		{
			SeatType = mapper.Map<SeatTypeDto>(seatType),
			Price = price
		};

		return selectedSeat;
	}
}