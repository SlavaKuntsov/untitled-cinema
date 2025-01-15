using MovieService.Domain.Models;

namespace MovieService.Domain.Entities;
public class SeatTypeEntity
{
	public Guid Id { get; set; }
	public string Name { get; set; } = null!;
	public decimal PriceModifier { get; set; } = 1;

	public virtual ICollection<SeatEntity> Seats { get; set; } = [];
}
