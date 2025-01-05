namespace MovieService.Domain.Entities;

public class MovieEntity
{
	public Guid Id { get; set; }
	public string Title { get; set; } = null!;
	public IList<string> Genres { get; set; } = [];
	public string Description { get; set; } = string.Empty;
	public short DurationMinutes { get; set; }
	public string Producer { get; set; } = null!;
	public DateTime ReleaseDate { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }

	public virtual ICollection<SessionEntity> Sessions { get; set; } = [];
}