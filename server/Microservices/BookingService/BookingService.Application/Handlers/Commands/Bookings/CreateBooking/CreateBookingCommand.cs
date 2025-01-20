using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CreateBooking;

public class CreateBookingCommand(Guid userId) : IRequest<Guid>
{
	public Guid UserId { get; private set; } = userId;
}