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
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<DayModel>> Handle(GetAllDaysQuery request, CancellationToken cancellationToken)
	{
		var halls = await _unitOfWork.Repository<DayEntity>().GetAsync(cancellationToken);

		return _mapper.Map<IList<DayModel>>(halls);
	}
}