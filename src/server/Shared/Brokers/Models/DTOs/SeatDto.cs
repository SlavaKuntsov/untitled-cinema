namespace Brokers.Models.DTOs;

public record SeatDto(
	Guid Id,
	int Row,
	int Column);