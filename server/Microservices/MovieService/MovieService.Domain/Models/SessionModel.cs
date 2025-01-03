namespace MovieService.Domain.Models;

public class SessionModel
{
	public Guid Id { get; set; }
	public Guid MovieId { get; set; }
	public Guid HallId { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }

	public MovieModel Movie { get; set; } = null!;
	public CinemaHallModel CinemaHall { get; set; } = null!;
}