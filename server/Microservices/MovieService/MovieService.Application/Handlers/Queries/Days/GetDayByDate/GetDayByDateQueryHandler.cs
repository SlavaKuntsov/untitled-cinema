using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Days.GetDayByDate;

public class GetDayByDateQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetDayByDateQuery, DayModel?>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<DayModel?> Handle(GetDayByDateQuery request, CancellationToken cancellationToken)
	{
		if (!request.Date.DateFormatTryParse(out DateTime parsedDate))
			throw new BadRequestException("Invalid date format.");

		var day = await _unitOfWork.DaysRepository.GetAsync(parsedDate, cancellationToken);

		return _mapper.Map<DayModel>(day);
	}
}