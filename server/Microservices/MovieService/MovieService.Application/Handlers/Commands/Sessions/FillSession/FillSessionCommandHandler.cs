using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain.Constants;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Sessoins.FillSession;

public class FillSessionCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<FillSessionCommand, Guid>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(FillSessionCommand request, CancellationToken cancellationToken)
	{
		if (!request.StartTime.DateTimeFormatTryParse(out DateTime parsedStartTime))
			throw new BadRequestException("Invalid date format.");

		var date = parsedStartTime.Date;

		var day = await _unitOfWork.DaysRepository.GetAsync(date, cancellationToken)
			?? throw new NotFoundException($"Day '{date.ToString(DateTimeConstants.DATE_FORMAT)}' doesn't exists");

		var movie = await _unitOfWork.MoviesRepository.GetAsync(request.MovieId, cancellationToken)
			?? throw new NotFoundException($"Movie with id {request.MovieId} doesn't exists");

		var hall = await _unitOfWork.HallsRepository.GetAsync(request.HallId, cancellationToken)
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

		var session = new SessionModel(
			Guid.NewGuid(),
			request.MovieId,
			request.HallId,
			day.Id,
			parsedStartTime,
			calculateEndTime);

		await _unitOfWork.SessionsRepository.CreateAsync(_mapper.Map<SessionEntity>(session), cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return session.Id;
	}
}