using BookingService.Domain.Models;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.PayBooking;

public class PayBookingCommand(
	Guid bookingId,
	Guid userId) : IRequest<BookingModel>
{
	public Guid BookingId { get; private set; } = bookingId;
	public Guid UserId { get; private set; } = userId;
}