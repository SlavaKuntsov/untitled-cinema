namespace MovieService.API.Contracts.Requests.Sessions;

public record FillSessionRequest(
	Guid MovieId,
	Guid HallId,
	string StartTime);