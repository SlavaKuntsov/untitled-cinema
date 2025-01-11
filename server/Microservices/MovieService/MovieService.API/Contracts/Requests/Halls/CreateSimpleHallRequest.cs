namespace MovieService.API.Contracts.Requests.Halls;

public record CreateSimpleHallRequest(
	string Name,
	short TotalSeats,
	byte Rows,
	byte Columns);