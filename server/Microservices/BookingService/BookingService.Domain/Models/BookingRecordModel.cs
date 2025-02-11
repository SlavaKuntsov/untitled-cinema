namespace BookingService.Domain.Models;

public class BookingRecordModel
{
	public Guid BookingId { get; private set; }
	public Guid SessionId { get; private set; }
	public Guid MovieId { get; private set; }
	public IList<SeatModel> Seats { get; private set; } = [];
	public decimal TotalPrice { get; private set; }
	public string Status { get; private set; } = null!;
	public DateTime CreatedAt { get; private set; }
}