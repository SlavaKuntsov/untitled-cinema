namespace MovieService.Domain;

public class MovieModel
{
	public Guid Id { get; set; }
	public string Title { get; set; } = null!;
	public IList<string> Genres { get; set; } = [];
	public string Description { get; set; } = string.Empty;
	public decimal Price { get; set; } = 0.00m;
	public short DurationMinutes { get; set; }
	public string Producer { get; set; } = null!;
	public DateTime ReleaseDate { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }

	public MovieModel()
	{
	}

	public MovieModel(
		Guid id,
		string title,
		string description,
		decimal price,
		short durationMinutes,
		string producer,
		DateTime releaseDate,
		DateTime createdAt,
		DateTime updatedAt)
	{
		Id = id;
		Title = title;
		Description = description;
		Price = price;
		DurationMinutes = durationMinutes;
		Producer = producer;
		ReleaseDate = releaseDate;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}