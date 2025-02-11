namespace Brokers.Models.Response;

public class BookingPriceResponse
{
	public decimal TotalPrice { get; private set; } = 0;
	public string Error { get; private set; } = string.Empty;

	public BookingPriceResponse(
		string error,
		decimal totalPrice = 0)
	{
		Error = error;
		TotalPrice = totalPrice;
	}
}