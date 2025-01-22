namespace BookingService.Domain.Models;

public class AvailableSeatsModel
{
	public Guid Id { get; private set; }
	public Guid SessionId { get; private set; }
	public List<SeatModel> AvailableSeatsList { get; private set; } = [];
	public List<SeatModel> ReservedSeats { get; private set; } = [];
	public DateTime UpdatedAt { get; private set; }
}