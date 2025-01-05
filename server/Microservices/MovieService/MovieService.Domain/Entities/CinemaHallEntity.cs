namespace MovieService.Domain.Entities;

public class CinemaHallEntity
{
	public Guid Id { get; private set; }
	public string Name { get; set; } = null!;
	public short TotalSeats { get; set; }

	public virtual ICollection<HallSeatEntity> Seats { get; set; } = [];
	public virtual ICollection<SessionEntity> Sessions { get; set; } = [];
}