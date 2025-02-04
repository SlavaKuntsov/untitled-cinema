using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;

using MediatR;

namespace BookingService.Application.Handlers.Query.Bookings.GetAllBookings;

public class GetAllBookingsQueryHandler(
	IBookingsRepository bookingsRepository) : IRequestHandler<GetAllBookingsQuery, IList<BookingEntity>>
{
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;

	public async Task<IList<BookingEntity>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
	{
		return await _bookingsRepository.GetAsync(cancellationToken);
	}
}