using BookingService.Domain.Entities;
using BookingService.Domain.Exceptions;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;

using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Seats.UpdateSeats;

public class UpdateSeatsCommandHandler(
	IRabbitMQProducer rabbitMQProducer,
	ISessionSeatsRepository sessionSeatsRepository,
	IMapper mapper) : IRequestHandler<UpdateSeatsCommand>
{
	private readonly IRabbitMQProducer _rabbitMQProducer = rabbitMQProducer;
	private readonly ISessionSeatsRepository _sessionSeatsRepository = sessionSeatsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(UpdateSeatsCommand request, CancellationToken cancellationToken)
	{
		bool isExist = true;

		var seatEntity = await _sessionSeatsRepository.GetAsync(
				s => s.SessionId == request.SessionId,
				cancellationToken);

		var sessionSeatsModel = _mapper.Map<SessionSeatsModel>(seatEntity);

		if (sessionSeatsModel is null)
		{
			isExist = false;

			var data = new SessionSeatsRequest(
				request.SessionId,
				request.Seats);

			var response = await _rabbitMQProducer.RequestReplyAsync<SessionSeatsRequest, SessionSeatsResponse>(
				data,
				Guid.NewGuid(),
				cancellationToken);

			if (!string.IsNullOrWhiteSpace(response.Error))
				throw new NotFoundException(response.Error);

			DateTime dateNow = DateTime.UtcNow;

			var newSessionSeatModel = new SessionSeatsModel(
				Guid.NewGuid(),
				request.SessionId,
				response.Seats,
				[],
				dateNow);

			sessionSeatsModel = newSessionSeatModel;
		}

		if (request.IsFromAvailableToReserved)
		{
			foreach (var seat in request.Seats)
			{
				var availableSeat = sessionSeatsModel.AvailableSeats.FirstOrDefault(s => s.Id == seat.Id);

				if (availableSeat is null)
					continue;

				sessionSeatsModel.AvailableSeats.Remove(availableSeat);
				sessionSeatsModel.ReservedSeats.Add(availableSeat);
			}
		}
		else
		{

			foreach (var seat in request.Seats)
			{
				var availableSeat = sessionSeatsModel.ReservedSeats.FirstOrDefault(s => s.Id == seat.Id);

				if (availableSeat is null)
					continue;

				sessionSeatsModel.ReservedSeats.Remove(availableSeat);
				sessionSeatsModel.AvailableSeats.Add(availableSeat);
			}
		}

		if (isExist)
		{
			await _sessionSeatsRepository.UpdateAsync(
				_mapper.Map<SessionSeatsEntity>(sessionSeatsModel),
				cancellationToken);
		}
		else
		{
			await _sessionSeatsRepository.CreateAsync(
				_mapper.Map<SessionSeatsEntity>(sessionSeatsModel),
				cancellationToken);
		}

		return;
	}
}