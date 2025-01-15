using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Enums;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Halls.CreateHall;

public class CreateCustomHallCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<CreateCustomHallCommand, Guid>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateCustomHallCommand request, CancellationToken cancellationToken)
	{
		foreach (var row in request.Seats)
		{
			if (row.Any(seat => !Enum.IsDefined(typeof(SeatType), seat)))
				throw new InvalidOperationException("Seats can only contain valid values from the SeatType enumeration.");
		}

		var seatCount = request.Seats.Sum(row =>
			row.Count(seat => seat != (int)SeatType.None));

		if (seatCount != request.TotalSeats)
			throw new InvalidOperationException(
				$"The number of seats ({seatCount}) does not match the specified total seats ({request.TotalSeats}).");

		var rowLength = request.Seats.FirstOrDefault()?.Length ?? 0;

		if (request.Seats.Any(row => row.Length != rowLength))
			throw new InvalidOperationException("All rows in seats must have the same length.");

		var existHall = await _unitOfWork.HallsRepository.GetAsync(request.Name, cancellationToken);

		if (existHall is not null)
			throw new AlreadyExistsException($"Hall with name '{request.Name}' already exist.");

		var hall = new HallModel(
			Guid.NewGuid(),
			request.Name,
			request.TotalSeats,
			request.Seats);

		await _unitOfWork.HallsRepository.CreateAsync(_mapper.Map<HallEntity>(hall), cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return hall.Id;
	}
}