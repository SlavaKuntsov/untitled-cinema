using System.ComponentModel;

namespace BookingService.Domain.Enums;

public enum BookingStatus
{
	[Description("cancelled")]
	Cancelled = -1,
	[Description("reserved")]
	Reserved = 0,
	[Description("paid")]
	Paid = 1,
}