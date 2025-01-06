using Mapster;

using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Halls.UpdateHall;

public class UpdateHallCommandHandler(
	IHallsRepository hallsRepository,
	IMapper mapper) : IRequestHandler<UpdateHallCommand, HallModel>
{
	private readonly IHallsRepository _hallsRepository = hallsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<HallModel> Handle(UpdateHallCommand request, CancellationToken cancellationToken)
	{
		var existHall = await _hallsRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Hall with id {request.Id} doesn't exists");

		request.Adapt(existHall);

		_hallsRepository.Update(existHall, cancellationToken);

		return _mapper.Map<HallModel>(existHall);
	}
}