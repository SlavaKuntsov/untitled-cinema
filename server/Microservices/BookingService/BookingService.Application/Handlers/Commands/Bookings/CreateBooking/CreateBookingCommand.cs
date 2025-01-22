using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CreateBooking;

public class CreateBookingCommand(
	Guid userId,
	Guid sessionId) : IRequest<Guid>
{
	public Guid UserId { get; private set; } = userId;
	public Guid SessionId { get; private set; } = sessionId;
}