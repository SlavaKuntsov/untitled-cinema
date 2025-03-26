using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Days.GetAllDays;

public class GetAllDaysQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllDaysQuery, IList<DayModel>>
{
	public async Task<IList<DayModel>> Handle(GetAllDaysQuery request, CancellationToken cancellationToken)
	{
		var halls = await unitOfWork.Repository<DayEntity>().GetAsync(cancellationToken);

		return mapper.Map<IList<DayModel>>(halls);
	}
}