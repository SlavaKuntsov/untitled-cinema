namespace BookingService.Domain.Entities;

public class AvailableSeatsEntity
{
	public Guid Id { get; set; }
	public Guid SessionId { get; set; }
	public List<SeatEntity> AvailableSeatsList { get; set; } = [];
	public List<SeatEntity> ReservedSeats { get; set; } = [];
	public DateTime UpdatedAt { get; set; }
}