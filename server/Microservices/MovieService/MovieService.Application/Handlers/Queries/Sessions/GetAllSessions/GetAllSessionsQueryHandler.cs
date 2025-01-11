using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Sessoins.GetAllSessions;

public class GetAllSessionsQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllSessionsQuery, IList<SessionModel>>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<SessionModel>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
	{
		var query = _unitOfWork.SessionsRepository.Get();

		if (!string.IsNullOrWhiteSpace(request.Date))
		{
			if (!request.Date.DateFormatTryParse(out DateTime parsedDate))
				throw new BadRequestException("Invalid date format.");

			var day = await _unitOfWork.DaysRepository.GetAsync(parsedDate, cancellationToken);

			if (day is not null)
				query = query.Where(s => s.DayId == day.Id);
			else
				query = query.Where(s => false);
		}

		if (!string.IsNullOrWhiteSpace(request.Hall))
		{
			var hall = await _unitOfWork.HallsRepository.GetAsync(request.Hall, cancellationToken);

			if (hall is not null)
				query = query.Where(s => s.HallId == hall.Id);
			else
				query = query.Where(s => false);
		}

		query = query
			.Skip((request.Offset - 1) * request.Limit)
			.Take(request.Limit);

		var sessions = await _unitOfWork.SessionsRepository.ToListAsync(query, cancellationToken);

		return _mapper.Map<IList<SessionModel>>(sessions);
	}
}