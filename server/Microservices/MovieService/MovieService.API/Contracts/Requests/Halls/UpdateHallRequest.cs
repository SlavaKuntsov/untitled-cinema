namespace MovieService.API.Contracts.Requests.Halls;

public record UpdateHallRequest(
    Guid id,
    string Name,
    short TotalSeats,
	int[][] Seats);