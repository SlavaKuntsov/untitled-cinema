namespace BookingService.Domain.Models;

public class AvailableSeatsModel
{
	public Guid Id { get; private set; }
	public Guid SessionId { get; private set; }
	public IList<SeatModel> AvailableSeats { get; private set; } = [];
	public IList<SeatModel> ReservedSeats { get; private set; } = [];
	public DateTime UpdatedAt { get; private set; }
}