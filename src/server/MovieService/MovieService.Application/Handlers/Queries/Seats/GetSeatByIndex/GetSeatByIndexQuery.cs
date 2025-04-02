using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using MovieService.Application.DTOs;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatByIndex;

public record GetSeatByIndexQuery(
	Guid HallId,
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
		var seat = await unitOfWork.SeatsRepository.GetAsync(
						request.HallId,
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

		var selectedSeat = mapper.Map<SelectedSeatDto>(seat);

		selectedSeat = selectedSeat with { SeatType = mapper.Map<SeatTypeModel>(seatType) };

		return selectedSeat;
	}
}