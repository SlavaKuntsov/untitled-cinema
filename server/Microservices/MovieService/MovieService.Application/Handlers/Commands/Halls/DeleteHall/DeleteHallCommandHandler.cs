using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Application.Handlers.Commands.Halls.DeleteHall;

public class DeleteHallCommandHandler(IHallsRepository hallsRepository, IMapper mapper) : IRequestHandler<DeleteHallCommand>
{
	private readonly IHallsRepository _hallsRepository = hallsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteHallCommand request, CancellationToken cancellationToken)
	{
		var hall = await _hallsRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Hall with id {request.Id} doesn't exists");

		_hallsRepository.Delete(hall);

		return;
	}
}