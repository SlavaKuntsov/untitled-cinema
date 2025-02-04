namespace Brokers.Models.Response;

public class BookingPayResponse
{
	public string Error { get; private set; } = string.Empty;

	public BookingPayResponse(string error)
	{
		Error = error;
	}
}