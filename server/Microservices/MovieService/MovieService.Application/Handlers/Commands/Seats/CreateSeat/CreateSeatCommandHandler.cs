using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Seats.CreateSeat;

public class CreateSeatCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<CreateSeatCommand, Guid>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateSeatCommand request, CancellationToken cancellationToken)
	{
		var existSeat = await _unitOfWork.SeatsRepository
			.GetAsync(request.Row, request.Column, cancellationToken);

		if (existSeat is not null)
			throw new AlreadyExistsException($"Seat with row {request.Row} and column {request.Column} already exist.");

		var hallEntity = await _unitOfWork.HallsRepository.GetAsync(request.Hall, cancellationToken)
			?? throw new NotFoundException($"Hall with name '{request.Hall}' not found.");

		var seatTypeEntity = await _unitOfWork.SeatsRepository.GetTypeAsync(request.SeatType, cancellationToken)
			?? throw new NotFoundException($"Seat type with name '{request.Hall}' not found.");

		var hall = _mapper.Map<HallModel>(hallEntity);

		if (request.Row < 0 || request.Row >= hall.SeatsArray.Length ||
			request.Column < 0 || request.Column >= hall.SeatsArray[request.Row].Length)
			throw new ArgumentOutOfRangeException($"Seat at row {request.Row} and column {request.Column} is out of bounds.");

		if (hall.SeatsArray[request.Row][request.Column] == -1)
			throw new InvalidOperationException($"Seat at row {request.Row} and column {request.Column} is not available.");

		var seatType = _mapper.Map<SeatTypeModel>(seatTypeEntity);

		var seat = new SeatModel(
			Guid.NewGuid(),
			hall.Id,
			seatType.Id,
			request.Row,
			request.Column);

		await _unitOfWork.SeatsRepository.CreateAsync(_mapper.Map<SeatEntity>(seat), cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return seat.Id;
	}
}