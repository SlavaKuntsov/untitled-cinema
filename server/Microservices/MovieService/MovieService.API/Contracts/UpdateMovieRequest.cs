namespace MovieService.API.Contracts;

public record UpdateMovieRequest(
	Guid id,
	string Title,
	IList<string> Genres,
	string Description,
	short DurationMinutes,
	string ReleaseDate);