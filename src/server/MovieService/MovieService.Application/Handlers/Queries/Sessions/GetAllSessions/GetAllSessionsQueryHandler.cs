using Domain.Exceptions;
using Extensions.Strings;
using MapsterMapper;
using MediatR;
using MovieService.Application.DTOs;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Queries.Sessions.GetAllSessions;

public class GetAllSessionsQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllSessionsQuery, IList<SessionWithHallDto>>
{
	public async Task<IList<SessionWithHallDto>> Handle(
		GetAllSessionsQuery request,
		CancellationToken cancellationToken)
	{
		var query = unitOfWork.SessionsRepository.Get();

		if (!string.IsNullOrWhiteSpace(request.Date))
		{
			if (!request.Date.DateFormatTryParse(out var parsedDate))
				throw new BadRequestException("Invalid date format.");

			var day = await unitOfWork.DaysRepository.GetAsync(parsedDate, cancellationToken);

			if (day is not null)
				query = query.Where(s => s.DayId == day.Id);
			else
				query = query.Where(s => false);
		}

		if (request.Movie != null)
			if (request.Movie.Value != Guid.Empty)
			{
				var movie = await unitOfWork.MoviesRepository.GetAsync(
					request.Movie.Value,
					cancellationToken);

				if (movie is not null)
					query = query.Where(s => s.MovieId == movie.Id);
				else
					query = query.Where(s => false);
			}

		if (!string.IsNullOrWhiteSpace(request.Hall))
		{
			var hall = await unitOfWork.HallsRepository.GetAsync(request.Hall, cancellationToken);

			if (hall is not null)
				query = query.Where(s => s.HallId == hall.Id);
			else
				query = query.Where(s => false);
		}

		query = query
			.Skip((request.Offset - 1) * request.Limit)
			.Take(request.Limit);

		var sessions = await unitOfWork.SessionsRepository.ToListAsync(query, cancellationToken);

		var sessionsWithHallDto = sessions.Select(
				s => new SessionWithHallDto(
					s.Id,
					s.MovieId,
					new HallDto(s.Hall.Id, s.Hall.Name),
					s.DayId,
					s.PriceModifier,
					s.StartTime,
					s.EndTime
				))
			.ToList();

		return sessionsWithHallDto;
	}
}