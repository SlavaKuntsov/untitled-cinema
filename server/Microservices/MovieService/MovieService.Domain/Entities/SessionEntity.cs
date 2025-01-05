namespace MovieService.Domain.Entities;

public class SessionEntity
{
	public Guid Id { get; set; }
	public Guid MovieId { get; set; }
	public Guid HallId { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }

	public MovieEntity Movie { get; set; } = null!;
	public CinemaHallEntity CinemaHall { get; set; } = null!;
}