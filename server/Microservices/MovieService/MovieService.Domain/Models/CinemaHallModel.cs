namespace MovieService.Domain.Models;

public class CinemaHallModel
{
	public Guid Id { get; private set; }
	public string Name { get; set; } = null!;
	public short TotalSeats { get; set; }

	public virtual ICollection<HallSeatModel> Seats { get; set; } = [];
	public virtual ICollection<SessionModel> Sessions { get; set; } = [];
}
