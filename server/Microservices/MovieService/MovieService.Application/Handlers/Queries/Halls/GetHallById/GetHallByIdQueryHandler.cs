using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Halls.GetHallById;

public class GetHallByIdQueryHandler(IHallsRepository hallsRepository, IMapper mapper) : IRequestHandler<GetHallByIdQuery, HallModel?>
{
	private readonly IHallsRepository _hallsRepository = hallsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<HallModel?> Handle(GetHallByIdQuery request, CancellationToken cancellationToken)
	{
		var hall = await hallsRepository.GetAsync(request.Id, cancellationToken);

		return _mapper.Map<HallModel>(hall);
	}
}