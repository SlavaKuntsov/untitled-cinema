using BookingService.Application.Handlers.Query.Bookings.GetBookingsByUserId;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Query.Bookings.GetUserBookings;

public class GetUserBookingsQueryHandler(
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<GetUserBookingsQuery, IList<BookingEntity>>
{
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<BookingEntity>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
	{
		return await _bookingsRepository.GetAsync(
			b => b.UserId == request.UserId,
			cancellationToken);
	}
}