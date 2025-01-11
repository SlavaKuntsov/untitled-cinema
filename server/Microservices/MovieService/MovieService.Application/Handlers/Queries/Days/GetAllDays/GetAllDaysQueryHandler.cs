using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Days.GetAllDays;

public class GetAllDaysQueryHandler(
	IDaysRepository daysRepository,
	IMapper mapper) : IRequestHandler<GetAllDaysQuery, IList<DayModel>>
{
	private readonly IDaysRepository _daysRepository = daysRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<DayModel>> Handle(GetAllDaysQuery request, CancellationToken cancellationToken)
	{
		var halls = await _daysRepository.GetAsync(cancellationToken);

		return _mapper.Map<IList<DayModel>>(halls);
	}
}