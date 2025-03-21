using MovieService.Domain.Models;

namespace MovieService.Domain.Entities;

public class SeatEntity
{
	public Guid Id { get; set; }
	public Guid HallId { get; set; }
	public Guid SeatTypeId { get; set; }
	public int Row { get; set; }
	public int Column { get; set; }

	public virtual HallEntity Hall { get; set; } = null!;
	public virtual SeatTypeEntity SeatType { get; set; } = null!;
}