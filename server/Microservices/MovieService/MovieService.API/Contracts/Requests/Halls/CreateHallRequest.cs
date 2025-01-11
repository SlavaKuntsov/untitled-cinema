namespace MovieService.API.Contracts.Requests.Halls;

public record CreateHallRequest(
    string Name,
    short TotalSeats,
    int[][] Seats);