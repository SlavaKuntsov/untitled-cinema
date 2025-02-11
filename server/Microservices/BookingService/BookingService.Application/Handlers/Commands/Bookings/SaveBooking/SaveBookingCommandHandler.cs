using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces.Repositories;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.SaveBooking;

public class SaveBookingCommandHandler(
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<SaveBookingCommand, Guid>
{
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(SaveBookingCommand request, CancellationToken cancellationToken)
	{
		await _bookingsRepository.CreateAsync(_mapper.Map<BookingEntity>(request.Booking), cancellationToken);

		return request.Booking.Id;
	}
}