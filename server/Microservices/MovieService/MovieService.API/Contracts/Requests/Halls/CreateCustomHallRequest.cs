namespace MovieService.API.Contracts.Requests.Halls;

public record CreateCustomHallRequest(
    string Name,
    short TotalSeats,
    int[][] Seats);