namespace MovieService.Domain.Entities.Movies;

public class MovieFrameEntity
{
	public Guid Id { get; set; }
	public byte[] Image { get; set; } = [];
	public Guid MovieId { get; set; }

	public virtual MovieEntity Movie { get; set; } = null!;
}