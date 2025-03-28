using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;
using Brokers.Interfaces;
using Brokers.Models.Request;
using Brokers.Models.Response;
using Domain.Exceptions;
using MapsterMapper;
using MediatR;

namespace BookingService.Application.Handlers.Commands.Seats.UpdateSeats;

public class UpdateSeatsCommandHandler(
	IRabbitMQProducer rabbitMQProducer,
	ISessionSeatsRepository sessionSeatsRepository,
	IMapper mapper) : IRequestHandler<UpdateSeatsCommand>
{
	public async Task Handle(UpdateSeatsCommand request, CancellationToken cancellationToken)
	{
		var isExist = true;

		var seatEntity = await sessionSeatsRepository.GetAsync(
			s => s.SessionId == request.SessionId,
			cancellationToken);

		var sessionSeatsModel = mapper.Map<SessionSeatsModel>(seatEntity);

		if (sessionSeatsModel is null)
		{
			isExist = false;

			var data = new SessionSeatsRequest(request.SessionId);

			var response =
				await rabbitMQProducer.RequestReplyAsync<SessionSeatsRequest, SessionSeatsResponse>(
					data,
					Guid.NewGuid(),
					cancellationToken);

			if (!string.IsNullOrWhiteSpace(response.Error))
				throw new NotFoundException(response.Error);

			var newSessionSeatModel = new SessionSeatsModel(
				Guid.NewGuid(),
				request.SessionId,
				response.Seats,
				[]);

			sessionSeatsModel = newSessionSeatModel;
		}

		if (request.IsFromAvailableToReserved)
			foreach (var seat in request.Seats)
			{
				var availableSeat = sessionSeatsModel.AvailableSeats.FirstOrDefault(s => s.Id == seat.Id);

				if (availableSeat is null)
					continue;

				sessionSeatsModel.AvailableSeats.Remove(availableSeat);
				sessionSeatsModel.ReservedSeats.Add(availableSeat);
			}
		else
			foreach (var seat in request.Seats)
			{
				var availableSeat = sessionSeatsModel.ReservedSeats.FirstOrDefault(s => s.Id == seat.Id);

				if (availableSeat is null)
					continue;

				sessionSeatsModel.ReservedSeats.Remove(availableSeat);
				sessionSeatsModel.AvailableSeats.Add(availableSeat);
			}

		if (isExist)
			await sessionSeatsRepository.UpdateAsync(
				mapper.Map<SessionSeatsEntity>(sessionSeatsModel),
				cancellationToken);
		else
			await sessionSeatsRepository.CreateAsync(
				mapper.Map<SessionSeatsEntity>(sessionSeatsModel),
				cancellationToken);
	}
}