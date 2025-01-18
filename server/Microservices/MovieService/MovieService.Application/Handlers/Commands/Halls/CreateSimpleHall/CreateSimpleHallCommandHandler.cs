using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Enums;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Halls.CreateSimpleHall;

public class CreateSimpleHallCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<CreateSimpleHallCommand, Guid>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateSimpleHallCommand request, CancellationToken cancellationToken)
	{
		var seatCount = request.Rows * request.Columns;

		if (seatCount != request.TotalSeats)
			throw new InvalidOperationException(
				$"The number of seats ({seatCount}) does not match the specified total seats ({request.TotalSeats}).");

		var existHall = await _unitOfWork.HallsRepository.GetAsync(request.Name, cancellationToken);

		if (existHall is not null)
			throw new AlreadyExistsException($"Hall with name '{request.Name}' already exist.");

		var seats = new int[request.Rows][];

		for (var i = 0; i < request.Rows; i++)
		{
			seats[i] = Enumerable.Repeat((int)SeatType.Standart, request.Columns).ToArray();
		}

		var hall = new HallModel(
			Guid.NewGuid(),
			request.Name,
			request.TotalSeats,
			seats);

		await _unitOfWork.Repository<HallEntity>().CreateAsync(_mapper.Map<HallEntity>(hall), cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return hall.Id;
	}
}