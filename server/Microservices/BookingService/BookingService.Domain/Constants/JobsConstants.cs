namespace BookingService.Domain.Constants;

public class JobsConstants
{
	public static readonly TimeSpan AFTER_BOOKING_EXPIRED = TimeSpan.FromHours(1);
	public static readonly TimeSpan AFTER_BOOKING_EXPIRED_TEST = TimeSpan.FromSeconds(10);
}
