namespace MovieService.API.Contracts.Requests.Sessions;

public record UpdateSessionRequest(
	Guid Id,
	Guid MovieId,
	Guid HallId,
	string StartTime);