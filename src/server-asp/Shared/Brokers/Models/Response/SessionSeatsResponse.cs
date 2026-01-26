using BookingService.Domain.Models;

namespace Brokers.Models.Response;

public record SessionSeatsResponse(
	string Error,
	IList<SeatModel>? Seats = null);