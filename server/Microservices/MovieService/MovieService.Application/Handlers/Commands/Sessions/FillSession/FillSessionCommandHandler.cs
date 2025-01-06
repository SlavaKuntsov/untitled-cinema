using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain.Constants;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Sessoins.FillSession;

public class FillSessionCommandHandler(
	ISessionsRepository sessionsRepository,
	IDaysRepository daysRepository,
	IMoviesRepository movieRepository,
	IMapper mapper) : IRequestHandler<FillSessionCommand, Guid>
{
	private readonly ISessionsRepository _sessionsRepository = sessionsRepository;
	private readonly IDaysRepository _daysRepository = daysRepository;
	private readonly IMoviesRepository _movieRepository = movieRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(FillSessionCommand request, CancellationToken cancellationToken)
	{
		if (!request.StartTime.DateTimeFormatTryParse(out DateTime parsedStartTime))
			throw new BadRequestException("Invalid date format.");

		var date = parsedStartTime.Date;

		var day = await _daysRepository.GetAsync(date, cancellationToken)
			?? throw new NotFoundException($"Day '{date.ToString(DateTimeConstants.DATE_FORMAT)}' doesn't exists");

		var movie = await _movieRepository.GetAsync(request.MovieId, cancellationToken)
			?? throw new NotFoundException($"Movie with id {request.MovieId} doesn't exists");

		var calculateEndTime = parsedStartTime.AddMinutes(movie.DurationMinutes);

		if (parsedStartTime < day.StartTime)
			throw new UnprocessableContentException("Session start time cannot be earlier than the start of the day.");

		if (calculateEndTime > day.EndTime)
			throw new UnprocessableContentException("Session end time cannot be later than the end of the day.");

		parsedStartTime = DateTime.SpecifyKind(parsedStartTime, DateTimeKind.Local).ToUniversalTime();
		calculateEndTime = DateTime.SpecifyKind(calculateEndTime, DateTimeKind.Local).ToUniversalTime();

		var sameExistSessions = await _sessionsRepository.GetOverlappingAsync(
			parsedStartTime,
			calculateEndTime,
			cancellationToken);

		if (sameExistSessions.Any())
		{
			var overlappingMovieIds = sameExistSessions.Select(s => s.MovieId).Distinct();
			throw new UnprocessableContentException(
				$"Conflicting sessions found for movies: {string.Join(", ", overlappingMovieIds)}");
		}

		var session = new SessionModel(
			Guid.NewGuid(),
			request.MovieId,
			request.HallId,
			day.Id,
			parsedStartTime,
			calculateEndTime);

		await _sessionsRepository.CreateAsync(_mapper.Map<SessionEntity>(session), cancellationToken);

		return session.Id;
	}
}