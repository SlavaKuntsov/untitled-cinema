using BookingService.Domain.Models;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CreateBooking;

public class CreateBookingCommand(
	Guid userId,
	Guid sessionId,
	IList<SeatModel> seats) : IRequest<Guid>
{
	public Guid UserId { get; private set; } = userId;
	public Guid SessionId { get; private set; } = sessionId;
	public IList<SeatModel> Seats { get; private set; } = seats;
}