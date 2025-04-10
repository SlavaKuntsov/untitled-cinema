
namespace MovieService.Domain.Entities.Movies;

public class MovieFrameEntity
{
	public Guid Id { get; set; } 
	public Guid MovieId { get; set; }
	public string FrameName { get; set; }
	public int Order { get; set; }

	public virtual MovieEntity Movie { get; set; } = null!;
}