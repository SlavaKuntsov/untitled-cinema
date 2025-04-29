using Brokers.Interfaces;
using Brokers.Models.DTOs;
using Brokers.Models.Request;
using Brokers.Models.Response;
using Domain.Constants;
using Domain.Exceptions;
using Extensions.Strings;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;
using Redis.Services;

namespace MovieService.Application.Handlers.Commands.Sessions.FillSession;

public class FillSessionCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService,
	IRabbitMQProducer rabbitMQProducer,
	IMapper mapper) : IRequestHandler<FillSessionCommand, Guid>
{
	public async Task<Guid> Handle(FillSessionCommand request, CancellationToken cancellationToken)
	{
		if (!request.StartTime.DateTimeFormatTryParse(out var parsedStartTime))
			throw new BadRequestException("Invalid date format.");

		var date = parsedStartTime.Date;

		var day = await unitOfWork.DaysRepository.GetAsync(date, cancellationToken)
				?? throw new NotFoundException(
					$"Day '{date.ToString(DateTimeConstants.DATE_FORMAT)}' doesn't exists");

		var movie = await unitOfWork.MoviesRepository.GetAsync(request.MovieId, cancellationToken)
					?? throw new NotFoundException($"Movie with id {request.MovieId} doesn't exists");

		var hall = await unitOfWork.Repository<HallEntity>().GetAsync(request.HallId, cancellationToken)
			?? throw new NotFoundException($"Hall with id {request.MovieId} doesn't exists");

		var calculateEndTime = parsedStartTime.AddMinutes(movie.DurationMinutes);

		if (parsedStartTime < day.StartTime)
			throw new UnprocessableContentException(
				"Session start time cannot be earlier than the start of the day.");

		if (calculateEndTime > day.EndTime)
			throw new UnprocessableContentException(
				"Session end time cannot be later than the end of the day.");

		var sameExistSessions = await unitOfWork.SessionsRepository.GetOverlappingAsync(
			hall.Id,
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
			request.PriceModifier,
			parsedStartTime,
			calculateEndTime);

		await unitOfWork.Repository<SessionEntity>()
			.CreateAsync(mapper.Map<SessionEntity>(session), cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		var availableSeats = await unitOfWork.SeatsRepository.GetBySessionIdAsync(
			session.Id,
			cancellationToken);

		var seats = mapper.Map<IList<SeatDto>>(availableSeats);

		var createSeatsData = new CreateSeatsRequest(session.Id, seats);

		var response = await
			rabbitMQProducer.RequestReplyAsync<CreateSeatsRequest, CreateSeatsResponse>(
				createSeatsData,
				Guid.NewGuid(),
				cancellationToken);
		
		if (!string.IsNullOrWhiteSpace(response.Error))
			throw new InvalidOperationException(response.Error);

		await redisCacheService.RemoveValuesByPatternAsync("movies_*");

		return session.Id;
	}
}