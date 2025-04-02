namespace BookingService.Application.DTOs;

public record UpdatedSeatDTO(
	Guid SessionId,
	Guid SeatId);