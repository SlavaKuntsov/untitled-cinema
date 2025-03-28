using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;
using MapsterMapper;
using MediatR;

namespace BookingService.Application.Handlers.Commands.Seats.CreateEmptySeats;

public record CreateEmptySeats(
	Guid SessionId,
	IList<SeatModel> AvailableSeats) : IRequest;

public sealed class CreateEmptySeatsHandler(
	ISessionSeatsRepository sessionSeatsRepository,
	IMapper mapper)
	: IRequestHandler<CreateEmptySeats>
{
	public async Task Handle(CreateEmptySeats request, CancellationToken cancellationToken)
	{
		var newSessionSeatModel = new SessionSeatsModel(
			Guid.NewGuid(),
			request.SessionId,
			request.AvailableSeats,
			[]);

		var newSessionSeatEntity = mapper.Map<SessionSeatsEntity>(newSessionSeatModel);

		await sessionSeatsRepository.CreateAsync(
			newSessionSeatEntity,
			cancellationToken);
	}
}