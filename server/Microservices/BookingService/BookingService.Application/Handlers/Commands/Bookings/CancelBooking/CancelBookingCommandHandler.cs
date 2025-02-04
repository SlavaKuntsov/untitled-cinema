using BookingService.Domain.Enums;
using BookingService.Domain.Extensions;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;

using MapsterMapper;

using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CancelBooking;

public class CancelBookingCommandHandler(
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<CancelBookingCommand, BookingModel>
{
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<BookingModel> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
	{
		var existBooking = await _bookingsRepository.GetOneAsync(
			b => b.Id == request.Id,
			cancellationToken);

		if (existBooking.Status == BookingStatus.Cancelled.GetDescription())
			throw new InvalidOperationException($"Booking with id '{existBooking.Id}' already cancelled.");

		await _bookingsRepository.UpdateStatusAsync(
			request.Id,
			BookingStatus.Cancelled.GetDescription(),
			cancellationToken);

		existBooking.Status = BookingStatus.Cancelled.GetDescription();

		return _mapper.Map<BookingModel>(existBooking);
	}
}