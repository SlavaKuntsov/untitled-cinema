using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Halls.GetAllHalls;

public class GetAllHallsQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllHallsQuery, IList<HallModel>>
{
	public async Task<IList<HallModel>> Handle(GetAllHallsQuery request, CancellationToken cancellationToken)
	{
		var halls = await unitOfWork.Repository<HallEntity>().GetAsync(cancellationToken);

		return mapper.Map<IList<HallModel>>(halls);
	}
}