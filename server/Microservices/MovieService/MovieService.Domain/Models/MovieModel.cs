namespace MovieService.Domain;

public class MovieModel
{
	public Guid Id { get; private set; }
	public string Title { get; private set; } = null!;
	public IList<string> Genres { get; private set; } = [];
	public string Description { get; private set; } = string.Empty;
	public decimal Price { get; private set; } = 0.00m;
	public short DurationMinutes { get; private set; }
	public string Producer { get; private set; } = null!;
	public byte AgeLimit { get; private set; }
	public DateTime ReleaseDate { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime UpdatedAt { get; private set; }

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
		byte ageLimit,
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
		AgeLimit = ageLimit;
		ReleaseDate = releaseDate;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}