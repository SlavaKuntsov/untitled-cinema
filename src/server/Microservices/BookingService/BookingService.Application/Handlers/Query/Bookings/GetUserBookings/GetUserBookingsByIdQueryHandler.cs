using BookingService.Application.Handlers.Query.Bookings.GetBookingsByUserId;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;
using MediatR;

namespace BookingService.Application.Handlers.Query.Bookings.GetUserBookings;

public class GetUserBookingsByIdQueryHandler(IBookingsRepository bookingsRepository)
	: IRequestHandler<GetUserBookingsByIdQuery, IList<BookingEntity>>
{
	public async Task<IList<BookingEntity>> Handle(
		GetUserBookingsByIdQuery request,
		CancellationToken cancellationToken)
	{
		return await bookingsRepository.GetAsync(
			b => b.UserId == request.UserId,
			cancellationToken);
	}
}