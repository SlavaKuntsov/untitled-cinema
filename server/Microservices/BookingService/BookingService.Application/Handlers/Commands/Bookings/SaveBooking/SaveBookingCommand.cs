using BookingService.Domain.Models;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.SaveBooking;

public class SaveBookingCommand(
	BookingModel booking) : IRequest<Guid>
{
	public BookingModel Booking { get; private set; } = booking;
}