using BookingService.Domain.Models;

namespace Brokers.Models.Request;

public record BookingPriceRequest(
	Guid SessionId,
	IList<SeatModel> Seats);