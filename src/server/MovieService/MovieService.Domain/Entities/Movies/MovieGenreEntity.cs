namespace MovieService.Domain.Entities.Movies;

public class MovieGenreEntity
{
	public Guid MovieId { get; set; }
	public MovieEntity Movie { get; set; } = null!;

	public Guid GenreId { get; set; }
	public GenreEntity Genre { get; set; } = null!;
}
