using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Halls.GetAllHalls;

public class GetAllHallsQueryHandler(
	IHallsRepository hallsRepository,
	IMapper mapper) : IRequestHandler<GetAllHallsQuery, IList<HallModel>>
{
	private readonly IHallsRepository _hallsRepository = hallsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<HallModel>> Handle(GetAllHallsQuery request, CancellationToken cancellationToken)
	{
		var halls = await _hallsRepository.GetAsync(cancellationToken);

		return _mapper.Map<IList<HallModel>>(halls);
	}
}