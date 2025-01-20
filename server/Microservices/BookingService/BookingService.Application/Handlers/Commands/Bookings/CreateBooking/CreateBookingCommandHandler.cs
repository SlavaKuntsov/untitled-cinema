using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Domain.Interfaces.Repositories;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CreateBooking;

public class CreateBookingCommandHandler(
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<CreateBookingCommand, Guid>
{
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
	{
		DateTime dateNow = DateTime.UtcNow;

		var booking = new BookingEntity(
			Guid.NewGuid(),
			Guid.NewGuid(),
			Guid.NewGuid(),
			[],
			10.0m,
			BookingStatus.Reserved,
			dateNow,
			dateNow);

		await _bookingsRepository.CreateAsync(booking, cancellationToken);

		return booking.Id;
	}
}