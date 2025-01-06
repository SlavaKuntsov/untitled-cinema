using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Halls.CreateHall;

public class CreateHallCommandHandler(IHallsRepository hallsRepository, IMapper mapper) : IRequestHandler<CreateHallCommand, Guid>
{
	private readonly IHallsRepository _hallsRepository = hallsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateHallCommand request, CancellationToken cancellationToken)
	{
		var existHall = await _hallsRepository.GetAsync(request.Name, cancellationToken);

		if (existHall is not null)
			throw new AlreadyExistsException($"Hall with name '{request.Name}' already exist.");

		var hall = new HallModel(
			Guid.NewGuid(),
			request.Name,
			request.TotalSeats);

		await _hallsRepository.CreateAsync(_mapper.Map<HallEntity>(hall), cancellationToken);

		return hall.Id;
	}
}