namespace Brokers.Models.Request;

public record BookingPayRequest(
	Guid UserId,
	decimal Price);