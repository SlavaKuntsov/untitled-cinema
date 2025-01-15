namespace MovieService.Domain.Entities;

public class HallEntity
{
	public Guid Id { get; private set; }
	public string Name { get; set; } = null!;
	public short TotalSeats { get; set; }
	public string SeatsArrayJson { get; set; } = "[]";


	public virtual ICollection<SeatEntity> Seats { get; set; } = [];
	public virtual ICollection<SessionEntity> Sessions { get; set; } = [];
}