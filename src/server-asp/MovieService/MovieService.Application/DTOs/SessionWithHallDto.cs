namespace MovieService.Application.DTOs;

public record SessionWithHallDto(
	Guid Id,
	Guid MovieId,
	HallDto Hall,
	Guid DayId,
	decimal PriceModifier,
	DateTime StartTime,
	DateTime EndTime);