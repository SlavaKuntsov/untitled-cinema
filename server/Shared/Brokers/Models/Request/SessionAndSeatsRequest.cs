using BookingService.Domain.Models;

namespace Brokers.Models.Request;

public class SessionAndSeatsRequest
{
	public Guid SessionId { get; private set; }
	public IList<SeatModel> Seats { get; private set; } = [];

	public SessionAndSeatsRequest(
		Guid sessionId,
		IList<SeatModel> seats)
	{
		SessionId = sessionId;
		Seats = seats;
	}
}
