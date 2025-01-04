namespace MovieService.API.Contracts;

public record CreateMovieRequest(
	string Title,
	IList<string> Genres,
	string Description,
	short DurationMinutes,
	string Producer,
	string ReleaseDate);