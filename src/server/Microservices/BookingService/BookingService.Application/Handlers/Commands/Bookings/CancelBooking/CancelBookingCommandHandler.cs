using BookingService.Domain.Enums;
using BookingService.Domain.Interfaces.Repositories;
using BookingService.Domain.Models;
using Extensions.Enums;
using MapsterMapper;
using MediatR;

namespace BookingService.Application.Handlers.Commands.Bookings.CancelBooking;

public class CancelBookingCommandHandler(
	IBookingsRepository bookingsRepository,
	IMapper mapper) : IRequestHandler<CancelBookingCommand, BookingModel>
{
	public async Task<BookingModel> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
	{
		var existBooking = await bookingsRepository.GetOneAsync(
			b => b.Id == request.Id,
			cancellationToken);

		if (existBooking.Status == BookingStatus.Cancelled.GetDescription())
			throw new InvalidOperationException($"Booking with id '{existBooking.Id}' already cancelled.");

		await bookingsRepository.UpdateStatusAsync(
			request.Id,
			BookingStatus.Cancelled.GetDescription(),
			cancellationToken);

		existBooking.Status = BookingStatus.Cancelled.GetDescription();

		return mapper.Map<BookingModel>(existBooking);
	}
}