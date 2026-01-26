namespace Brokers.Models.Response;

public record BookingPriceResponse(
	string Error,
	decimal TotalPrice = 0);