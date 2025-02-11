using BookingService.Domain.Models;

namespace Brokers.Models.Response;

public class SessionSeatsResponse
{
	public IList<SeatModel> Seats { get; private set; } = [];
	public string Error { get; private set; } = string.Empty;

	public SessionSeatsResponse(
		string error,
		IList<SeatModel>? seats = null)
	{
		Error = error;
		Seats = seats ?? [];
	}
}