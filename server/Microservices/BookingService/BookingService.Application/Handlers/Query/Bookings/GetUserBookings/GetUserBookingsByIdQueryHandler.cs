using BookingService.Application.Handlers.Query.Bookings.GetBookingsByUserId;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;

using MediatR;

namespace BookingService.Application.Handlers.Query.Bookings.GetUserBookings;

public class GetUserBookingsByIdQueryHandler(
	IBookingsRepository bookingsRepository) : IRequestHandler<GetUserBookingsByIdQuery, IList<BookingEntity>>
{
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;

	public async Task<IList<BookingEntity>> Handle(GetUserBookingsByIdQuery request, CancellationToken cancellationToken)
	{
		return await _bookingsRepository.GetAsync(
			b => b.UserId == request.UserId,
			cancellationToken);
	}
}