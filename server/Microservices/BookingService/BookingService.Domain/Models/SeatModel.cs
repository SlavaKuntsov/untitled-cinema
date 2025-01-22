namespace BookingService.Domain.Models;

public class SeatModel
{
	public Guid Id { get; private set; }
	public int Row { get; private set; }
	public int Column { get; private set; }
}