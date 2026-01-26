using Domain.Exceptions;
using Extensions.Enums;
using MapsterMapper;
using MediatR;
using MovieService.Application.Handlers.Commands.Halls.CreateHall;
using MovieService.Domain.Entities;
using MovieService.Domain.Enums;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Halls.CreateCustomHall;

public class CreateCustomHallCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<CreateCustomHallCommand, Guid>
{
	public async Task<Guid> Handle(CreateCustomHallCommand request, CancellationToken cancellationToken)
	{
		foreach (var row in request.Seats)
			if (row.Any(seat => !Enum.IsDefined(typeof(SeatType), seat)))
				throw new InvalidOperationException(
					"Seats can only contain valid values from the SeatType values.");

		var seatCount = request.Seats.Sum(
			row =>
				row.Count(seat => seat != (int)SeatType.None));

		if (seatCount != request.TotalSeats)
			throw new InvalidOperationException(
				$"The number of seats ({seatCount}) does not match the specified total seats ({request.TotalSeats}).");

		var rowLength = request.Seats.FirstOrDefault()?.Length ?? 0;

		if (request.Seats.Any(row => row.Length != rowLength))
			throw new InvalidOperationException("All rows in seats must have the same length.");

		var existHall = await unitOfWork.HallsRepository.GetAsync(request.Name, cancellationToken);

		if (existHall is not null)
			throw new AlreadyExistsException($"Hall with name '{request.Name}' already exist.");

		var hall = new HallModel(
			Guid.NewGuid(),
			request.Name,
			request.TotalSeats,
			request.Seats);

		var seatModels = new List<SeatModel>();

		for (var row = 0; row < hall.SeatsArray.Length; row++)
		for (var column = 0; column < hall.SeatsArray[row].Length; column++)
		{
			var seatType = (SeatType)hall.SeatsArray[row][column];

			if (seatType == SeatType.None)
				continue;

			var seatTypeDescription = seatType.GetDescription();

			var seatTypeEntity = await unitOfWork.SeatsRepository
									.GetTypeAsync(seatTypeDescription, cancellationToken)
								?? throw new NotFoundException(
									$"Seat type with name '{seatTypeDescription}' doesn't exists");

			var seat = new SeatModel(
				Guid.NewGuid(),
				hall.Id,
				seatTypeEntity.Id,
				row + 1,
				column + 1
			);

			seatModels.Add(seat);
		}

		await unitOfWork.Repository<HallEntity>()
			.CreateAsync(mapper.Map<HallEntity>(hall), cancellationToken);

		await unitOfWork.SeatsRepository
			.CreateRangeAsync(mapper.Map<IList<SeatEntity>>(seatModels), cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return hall.Id;
	}
}