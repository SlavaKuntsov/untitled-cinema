using MovieService.Domain.Models;

namespace MovieService.Domain;

public class MovieModel
{
	public Guid Id { get; set; }
	public string Title { get; set; } = null!;
	public IList<string> Genres { get; set; } = [];
	public string Description { get; set; } = string.Empty;
	public int DurationMinutes { get; set; }
	public DateTime ReleaseDate { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }

	public virtual ICollection<SessionModel> Sessions { get; set; } = [];
}