using Brokers.Models.DTOs;

namespace Brokers.Models.Request;

public record CreateSeatsRequest(
	Guid SessionId,
	IList<SeatDto> Seats);