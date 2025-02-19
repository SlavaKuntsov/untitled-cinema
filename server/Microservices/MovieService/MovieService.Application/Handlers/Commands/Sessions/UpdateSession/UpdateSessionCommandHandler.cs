using Mapster;

using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Application.Interfaces.Caching;
using MovieService.Domain.Constants;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Sessions.UpdateSession;

public class UpdateSessionCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<UpdateSessionCommand, SessionModel>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IRedisCacheService _redisCacheService = redisCacheService;
	private readonly IMapper _mapper = mapper;

	public async Task<SessionModel> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
	{
		var existSession = await _unitOfWork.Repository<SessionEntity>().GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Session with id {request.Id} doesn't exists");

		if (!request.StartTime.DateTimeFormatTryParse(out DateTime parsedStartTime))
			throw new BadRequestException("Invalid date format.");

		var date = parsedStartTime.Date;

		var day = await _unitOfWork.DaysRepository.GetAsync(date, cancellationToken)
			?? throw new NotFoundException($"Day '{date.ToString(DateTimeConstants.DATE_FORMAT)}' doesn't exists");

		var movie = await _unitOfWork.MoviesRepository.GetAsync(request.MovieId, cancellationToken)
			?? throw new NotFoundException($"Movie with id {request.MovieId} doesn't exists");

		var hall = await _unitOfWork.Repository<HallEntity>().GetAsync(request.HallId, cancellationToken)
			?? throw new NotFoundException($"Hall with id {request.MovieId} doesn't exists");

		var calculateEndTime = parsedStartTime.AddMinutes(movie.DurationMinutes);

		if (parsedStartTime < day.StartTime)
			throw new UnprocessableContentException("Session start time cannot be earlier than the start of the day.");

		if (calculateEndTime > day.EndTime)
			throw new UnprocessableContentException("Session end time cannot be later than the end of the day.");

		var sameExistSessions = await _unitOfWork.SessionsRepository.GetOverlappingAsync(
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

		_unitOfWork.Repository<SessionEntity>().Update(existSession);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		await _redisCacheService.RemoveValuesByPatternAsync("movies_*");

		return _mapper.Map<SessionModel>(existSession);
	}
}