using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;
using Domain.Exceptions;
using MapsterMapper;
using MediatR;

namespace BookingService.Application.Handlers.Query.Seats.GetSeatsById;

public class GetSeatsBySessionIdQueryHandler(
	ISessionSeatsRepository seatsRepository,
	IMapper mapper) : IRequestHandler<GetSeatsBySessionIdQuery, SessionSeatsModel>
{
	public async Task<SessionSeatsModel> Handle(GetSeatsBySessionIdQuery request, CancellationToken cancellationToken)
	{
		var seat = await seatsRepository.GetAsync(
						b => b.SessionId == request.SessionId,
						request.IsAvailable,
						cancellationToken)
					?? throw new NotFoundException("Session seat not found");

		return mapper.Map<SessionSeatsModel>(seat);
	}
}