using Domain.Constants;
using Domain.Exceptions;
using Extensions.Strings;
using Mapster;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;
using Redis.Services;

namespace MovieService.Application.Handlers.Commands.Sessions.UpdateSession;

public class UpdateSessionCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<UpdateSessionCommand, SessionModel>
{
	public async Task<SessionModel> Handle(UpdateSessionCommand request, CancellationToken cancellationToken)
	{
		var existSession = await unitOfWork.Repository<SessionEntity>().GetAsync(request.Id, cancellationToken)
							?? throw new NotFoundException($"Session with id {request.Id} doesn't exists");

		if (!request.StartTime.DateTimeFormatTryParse(out var parsedStartTime))
			throw new BadRequestException("Invalid date format.");

		var date = parsedStartTime.Date;

		var day = await unitOfWork.DaysRepository.GetAsync(date, cancellationToken)
				?? throw new NotFoundException($"Day '{date.ToString(DateTimeConstants.DATE_FORMAT)}' doesn't exists");

		var movie = await unitOfWork.MoviesRepository.GetAsync(request.MovieId, cancellationToken)
					?? throw new NotFoundException($"Movie with id {request.MovieId} doesn't exists");

		var hall = await unitOfWork.Repository<HallEntity>().GetAsync(request.HallId, cancellationToken)
			?? throw new NotFoundException($"Hall with id {request.MovieId} doesn't exists");

		var calculateEndTime = parsedStartTime.AddMinutes(movie.DurationMinutes);

		if (parsedStartTime < day.StartTime)
			throw new UnprocessableContentException("Session start time cannot be earlier than the start of the day.");

		if (calculateEndTime > day.EndTime)
			throw new UnprocessableContentException("Session end time cannot be later than the end of the day.");

		var sameExistSessions = await unitOfWork.SessionsRepository.GetOverlappingAsync(
			hall.Id,
			parsedStartTime,
			calculateEndTime,
			cancellationToken);

		if (sameExistSessions.Count > 1)
			if (sameExistSessions.Any())
			{
				var overlappingMovieIds = sameExistSessions.Select(s => s.MovieId).Distinct();

				throw new UnprocessableContentException(
					$"Conflicting sessions found for movies: {string.Join(", ", overlappingMovieIds)}");
			}

		request.Adapt(existSession);

		unitOfWork.Repository<SessionEntity>().Update(existSession);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		await redisCacheService.RemoveValuesByPatternAsync("movies_*");

		return mapper.Map<SessionModel>(existSession);
	}
}