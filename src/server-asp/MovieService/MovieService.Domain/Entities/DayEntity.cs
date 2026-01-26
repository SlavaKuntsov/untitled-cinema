namespace MovieService.Domain.Entities;

public class DayEntity
{
	public Guid Id { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }

	public virtual ICollection<SessionEntity> Sessions { get; set; } = null!;
}