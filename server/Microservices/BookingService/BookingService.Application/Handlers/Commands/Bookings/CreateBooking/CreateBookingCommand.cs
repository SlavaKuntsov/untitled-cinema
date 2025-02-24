using BookingService.Domain.Models;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CreateBooking;

public record struct CreateBookingCommand(
	Guid? UserId,
	Guid SessionId,
	IList<SeatModel> Seats) : IRequest<Guid>;