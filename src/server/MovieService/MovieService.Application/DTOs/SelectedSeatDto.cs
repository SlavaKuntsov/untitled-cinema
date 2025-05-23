namespace MovieService.Application.DTOs;

public record struct SelectedSeatDto(
	Guid Id,
	int Row,
	int Column,
	decimal Price,
	SeatTypeDto SeatType);