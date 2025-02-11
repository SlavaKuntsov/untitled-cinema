namespace BookingService.Domain.Entities;

public class SeatEntity
{
	public Guid Id { get; set; }
	public int Row { get; set; }
	public int Column { get; set; }
}