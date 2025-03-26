namespace Brokers.Models.Request;

public record BookingPriceRequest<T>(
	Guid SessionId,
	IList<T> Seats);