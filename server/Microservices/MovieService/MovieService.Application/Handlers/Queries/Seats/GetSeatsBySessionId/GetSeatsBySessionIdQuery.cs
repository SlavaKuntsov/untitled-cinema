using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetAllSeatById;

public class GetSeatsBySessionIdQuery(Guid sessionId) : IRequest<IList<SeatModel>>
{
	public Guid SessionId { get; private set; } = sessionId;
}