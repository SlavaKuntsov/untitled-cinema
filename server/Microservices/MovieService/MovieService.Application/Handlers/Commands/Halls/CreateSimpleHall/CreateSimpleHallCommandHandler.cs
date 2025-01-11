using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Halls.CreateSimpleHall;

public class CreateSimpleHallCommandHandler(
	IHallsRepository hallsRepository,
	IMapper mapper) : IRequestHandler<CreateSimpleHallCommand, Guid>
{
	private readonly IHallsRepository _hallsRepository = hallsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateSimpleHallCommand request, CancellationToken cancellationToken)
	{
		var seatCount = request.Rows * request.Columns;

		if (seatCount != request.TotalSeats)
			throw new InvalidOperationException(
				$"The number of seats ({seatCount}) does not match the specified total seats ({request.TotalSeats}).");

		var existHall = await _hallsRepository.GetAsync(request.Name, cancellationToken);

		if (existHall is not null)
			throw new AlreadyExistsException($"Hall with name '{request.Name}' already exist.");

		var seats = new int[request.Rows][];

		for (var i = 0; i < request.Rows; i++)
		{
			seats[i] = Enumerable.Repeat(0, request.Columns).ToArray();
		}

		var hall = new HallModel(
			Guid.NewGuid(),
			request.Name,
			request.TotalSeats,
			seats);

		await _hallsRepository.CreateAsync(_mapper.Map<HallEntity>(hall), cancellationToken);

		return hall.Id;
	}
}