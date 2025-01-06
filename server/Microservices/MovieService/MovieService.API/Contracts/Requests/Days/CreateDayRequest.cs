namespace MovieService.API.Contracts.Requests.Days;

public record CreateDayRequest(
	string StartTime,
	string EndTime);