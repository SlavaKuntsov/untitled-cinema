using MovieService.Domain.Entities.Movies;

namespace MovieService.Domain.Entities;

public class GenreEntity
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;

	public virtual ICollection<MovieGenreEntity> MovieGenres { get; set; } = [];
}