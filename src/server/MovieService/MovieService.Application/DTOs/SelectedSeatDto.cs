using MovieService.Domain.Models;

namespace MovieService.Application.DTOs;

public record struct SelectedSeatDto(
	Guid Id,
	int Row,
	int Column,
	SeatTypeModel SeatType);