using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Query.Bookings.GetAllBookings;

public class GetAllBookingsQueryHandler(
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<GetAllBookingsQuery, IList<BookingEntity>>
{
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<BookingEntity>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
	{
		return await _bookingsRepository.GetAsync(cancellationToken);
	}
}