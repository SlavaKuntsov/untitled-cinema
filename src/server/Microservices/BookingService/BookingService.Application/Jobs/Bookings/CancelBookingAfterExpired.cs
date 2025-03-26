using BookingService.Domain.Enums;
using BookingService.Domain.Interfaces.Repositories;
using Extensions.Enums;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Jobs.Bookings;

public class CancelBookingAfterExpired(
	IBookingsRepository bookingsRepository,
	ILogger<CancelBookingAfterExpired> logger)
{
	public async Task ExecuteAsync(Guid bookingId, CancellationToken cancellationToken)
	{
		logger.LogInformation($"CancelBookingAfterExpired job for '{bookingId}' started");

		// Change booking status for the canceled if it was not paid 
		await bookingsRepository.UpdateStatusAsync(
			bookingId,
			BookingStatus.Cancelled.GetDescription(),
			cancellationToken);
		//

		logger.LogInformation($"CancelBookingAfterExpired job for '{bookingId}' finished");
	}
}