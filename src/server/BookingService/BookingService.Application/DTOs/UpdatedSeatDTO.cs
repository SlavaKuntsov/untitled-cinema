using BookingService.Domain.Models;

namespace BookingService.Application.DTOs;

public record UpdatedSeatDTO(
	Guid SessionId,
	SeatModel Seat);