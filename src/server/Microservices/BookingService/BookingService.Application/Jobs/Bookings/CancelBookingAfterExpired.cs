using BookingService.Domain.Enums;
using BookingService.Domain.Extensions;
using BookingService.Domain.Interfaces.Repositories;

using Microsoft.Extensions.Logging;

namespace BookingService.Application.Jobs.Bookings;

public class CancelBookingAfterExpired(
	IBookingsRepository bookingsRepository,
	ILogger<CancelBookingAfterExpired> logger)
{
	private readonly IBookingsRepository _bookingsRepository = bookingsRepository;
	private readonly ILogger<CancelBookingAfterExpired> _logger = logger;

	public async Task ExecuteAsync(Guid bookingId, CancellationToken cancellationToken)
	{
		_logger.LogInformation($"CancelBookingAfterExpired job for '{bookingId}' started");

		// Change booking status for the canceled if it was not paid 
		await _bookingsRepository.UpdateStatusAsync(
			bookingId,
			BookingStatus.Cancelled.GetDescription(),
			cancellationToken);
		//

		_logger.LogInformation($"CancelBookingAfterExpired job for '{bookingId}' finished");

		return;
	}
}