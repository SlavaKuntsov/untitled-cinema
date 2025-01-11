namespace MovieService.API.Contracts.Requests.Movies;

public record UpdateMovieRequest(
    Guid Id,
    string Title,
    IList<string> Genres,
    string Description,
    short DurationMinutes,
    string Producer,
    string ReleaseDate);