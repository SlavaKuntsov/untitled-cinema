using MovieService.Domain.Entities.Movies;

namespace MovieService.Domain.Entities;

public class SessionEntity
{
	public Guid Id { get; set; }
	public Guid MovieId { get; set; }
	public Guid HallId { get; set; }
	public Guid DayId { get; set; }
	public decimal PriceModifier { get; set; } = 1;
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }

	public MovieEntity Movie { get; set; } = null!;
	public HallEntity Hall { get; set; } = null!;
	public DayEntity Day { get; set; } = null!;
}