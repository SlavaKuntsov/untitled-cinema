using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Seats.CreateSeat;

public class CreateSeatCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<CreateSeatCommand, Guid>
{
	public async Task<Guid> Handle(CreateSeatCommand request, CancellationToken cancellationToken)
	{
		var existSeat = await unitOfWork.SeatsRepository.GetAsync(
			request.HallId,
			request.Row,
			request.Column,
			cancellationToken);

		if (existSeat is not null)
			throw new AlreadyExistsException(
				$"Seat with row {request.Row} and column {request.Column} already exist.");

		var hallEntity = await unitOfWork.Repository<HallEntity>().GetAsync(request.HallId, cancellationToken)
						?? throw new NotFoundException($"Hall with id '{request.HallId}' not found.");

		var seatTypeEntity =
			await unitOfWork.SeatsRepository.GetTypeAsync(request.SeatType, cancellationToken)
			?? throw new NotFoundException($"Seat type with name '{request.SeatType}' not found.");

		var hall = mapper.Map<HallModel>(hallEntity);

		if (request.Row < 0 || request.Row >= hall.SeatsArray.Length ||
			request.Column < 0 || request.Column >= hall.SeatsArray[request.Row].Length)
			throw new ArgumentOutOfRangeException(
				$"Seat at row {request.Row} and column {request.Column} is out of bounds.");

		if (hall.SeatsArray[request.Row][request.Column] == -1)
			throw new InvalidOperationException(
				$"Seat at row {request.Row} and column {request.Column} is not available.");

		var seatType = mapper.Map<SeatTypeModel>(seatTypeEntity);

		var seat = new SeatModel(
			Guid.NewGuid(),
			hall.Id,
			seatType.Id,
			request.Row,
			request.Column);

		await unitOfWork.Repository<SeatEntity>()
			.CreateAsync(mapper.Map<SeatEntity>(seat), cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return seat.Id;
	}
}