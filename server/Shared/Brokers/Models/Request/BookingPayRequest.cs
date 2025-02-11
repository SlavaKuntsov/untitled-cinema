namespace Brokers.Models.Request;

public class BookingPayRequest
{
	public Guid UserId { get; private set; }
	public decimal Price { get; private set; }

	public BookingPayRequest(
		Guid userId, 
		decimal price)
	{
		UserId = userId;
		Price = price;
	}
}