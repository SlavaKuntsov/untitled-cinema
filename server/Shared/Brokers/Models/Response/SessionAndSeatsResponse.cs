namespace Brokers.Models.Response;

public class SessionAndSeatsResponse
{
	public decimal TotalPrice { get; private set; } = 0;
	public string Error { get; private set; } = string.Empty;

	public SessionAndSeatsResponse(
		string error,
		decimal totalPrice = 0)
	{
		Error = error;
		TotalPrice = totalPrice;
	}
}