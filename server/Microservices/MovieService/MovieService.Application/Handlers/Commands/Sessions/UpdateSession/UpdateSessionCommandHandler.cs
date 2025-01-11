using Mapster;

using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain.Constants;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Sessions.UpdateSession;

public class UpdateSessionCommandHandler(
	ISessionsRepository sessionsRepository,
	IDaysRepository daysRepository,
	IMoviesRepository movieRepository,
	IHallsRepository hallsRepository,
	IMapper mapper) : IRequestHandler<UpdateSessionCommand, SessionModel>
{
	private readonly ISessionsRepository _sessionsRepository = sessionsRepository;
	private readonly IDaysRepository _daysRepository = daysRepository;
	private readonly IMoviesRepository _moviesRepository = movieRepository;
	private readonly IHallsRepository _hallsRepository = hallsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<SessionModel> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
	{
		var existSession = await _sessionsRepository.GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Session with id {request.Id} doesn't exists");

		if (!request.StartTime.DateTimeFormatTryParse(out DateTime parsedStartTime))
			throw new BadRequestException("Invalid date format.");

		var date = parsedStartTime.Date;

		var day = await _daysRepository.GetAsync(date, cancellationToken)
			?? throw new NotFoundException($"Day '{date.ToString(DateTimeConstants.DATE_FORMAT)}' doesn't exists");

		var movie = await _moviesRepository.GetAsync(request.MovieId, cancellationToken)
			?? throw new NotFoundException($"Movie with id {request.MovieId} doesn't exists");

		var hall = await _hallsRepository.GetAsync(request.HallId, cancellationToken)
			?? throw new NotFoundException($"Hall with id {request.MovieId} doesn't exists");

		var calculateEndTime = parsedStartTime.AddMinutes(movie.DurationMinutes);

		if (parsedStartTime < day.StartTime)
			throw new UnprocessableContentException("Session start time cannot be earlier than the start of the day.");

		if (calculateEndTime > day.EndTime)
			throw new UnprocessableContentException("Session end time cannot be later than the end of the day.");

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

		request.Adapt(existSession);

		_sessionsRepository.Update(existSession, cancellationToken);

		return _mapper.Map<SessionModel>(existSession);
	}
}