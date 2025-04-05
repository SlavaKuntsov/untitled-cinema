using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;
using MediatR;
using Serilog;

namespace BookingService.Application.Handlers.Query.Bookings.GetAllBookings;

public class GetAllBookingsQueryHandler(IBookingsRepository bookingsRepository)
	: IRequestHandler<GetAllBookingsQuery, IList<BookingEntity>>
{
	public async Task<IList<BookingEntity>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
	{
		return await bookingsRepository.GetAsync(cancellationToken);
	}
}