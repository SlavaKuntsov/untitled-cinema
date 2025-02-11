using BookingService.Domain.Models;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CancelBooking;

public class CancelBookingCommand(Guid id) : IRequest<BookingModel>
{
	public Guid Id { get; private set; } = id;
}