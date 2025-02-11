namespace BookingService.Domain.Entities;

public class BookingRecordEntity
{
	public Guid BookingId { get; set; }
	public Guid SessionId { get; set; }
	public Guid MovieId { get; set; }
	public IList<SeatEntity> Seats { get; set; } = [];
	public decimal TotalPrice { get; set; }
	public string Status { get; set; } = null!;
	public DateTime CreatedAt { get; set; }
}