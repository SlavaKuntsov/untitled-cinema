namespace BookingService.Domain.Constants;

public class JobsConstants
{
	public static readonly TimeSpan AfterBookingExpired = TimeSpan.FromHours(1);
	public static readonly TimeSpan AfterBookingExpiredTest = TimeSpan.FromSeconds(10);
}
