namespace Brokers.Models.Response;

public record SessionSeatsResponse<T>(
	string Error,
	IList<T>? Seats = null);