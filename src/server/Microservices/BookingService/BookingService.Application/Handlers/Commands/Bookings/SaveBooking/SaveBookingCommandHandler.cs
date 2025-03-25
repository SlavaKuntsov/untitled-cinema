using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;
using MapsterMapper;
using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.SaveBooking;

public class SaveBookingCommandHandler(
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<SaveBookingCommand, Guid>
{
	public async Task<Guid> Handle(SaveBookingCommand request, CancellationToken cancellationToken)
	{
		await bookingsRepository.CreateAsync(
			mapper.Map<BookingEntity>(request.Booking),
			cancellationToken);

		return request.Booking.Id;
	}
}