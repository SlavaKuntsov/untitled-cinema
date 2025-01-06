using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Days.GetDayByDate;

public class GetDayByDateQueryHandler(IDaysRepository daysRepository, IMapper mapper) : IRequestHandler<GetDayByDateQuery, DayModel?>
{
	private readonly IDaysRepository _daysRepository = daysRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<DayModel?> Handle(GetDayByDateQuery request, CancellationToken cancellationToken)
	{
		if (!request.Date.DateFormatTryParse(out DateTime parsedDate))
			throw new BadRequestException("Invalid date format.");

		var movie = await _daysRepository.GetAsync(parsedDate, cancellationToken);

		return _mapper.Map<DayModel>(movie);
	}
}