namespace MovieService.API.Contracts.Requests.Halls;

public record UpdateHallRequest(
    Guid Id,
    string Name,
    short TotalSeats,
	int[][] Seats);