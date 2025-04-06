using Domain.Exceptions;
using Extensions.Strings;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Days.GetDayByDate;

public class GetDayByDateQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetDayByDateQuery, DayModel?>
{
	public async Task<DayModel?> Handle(GetDayByDateQuery request, CancellationToken cancellationToken)
	{
		if (!request.Date.DateFormatTryParse(out var parsedDate))
			throw new BadRequestException("Invalid date format.");

		var day = await unitOfWork.DaysRepository.GetAsync(parsedDate, cancellationToken)
				?? throw new NotFoundException($"Day '{request.Date}' not found.");

		return mapper.Map<DayModel>(day);
	}
}