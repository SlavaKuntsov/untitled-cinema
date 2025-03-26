namespace BookingService.Domain.Entities;

public class SessionSeatsEntity
{
	public Guid Id { get; set; }
	public Guid SessionId { get; set; }
	public IList<SeatEntity> AvailableSeats { get; set; } = [];
	public IList<SeatEntity> ReservedSeats { get; set; } = [];
	public DateTime UpdatedAt { get; set; }
}