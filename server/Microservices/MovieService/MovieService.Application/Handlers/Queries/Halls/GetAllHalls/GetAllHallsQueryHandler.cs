using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Halls.GetAllHalls;

public class GetAllHallsQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllHallsQuery, IList<HallModel>>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<HallModel>> Handle(GetAllHallsQuery request, CancellationToken cancellationToken)
	{
		var halls = await _unitOfWork.HallsRepository.GetAsync(cancellationToken);

		return _mapper.Map<IList<HallModel>>(halls);
	}
}