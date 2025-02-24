using BookingService.Domain.Models;

using MediatR;

namespace BookingService.Application.Handlers.Query.Seats.GetSeatsById;

public class GetSeatsByIdQuery(
	Guid sessionId,
	bool isAvailable) : IRequest<SessionSeatsModel>
{
	public Guid SessionId { get; private set; } = sessionId;
	public bool IsAvailable { get; private set; } = isAvailable;
}