namespace MovieService.Domain.Models;

public class SeatModel
{
	public Guid Id { get; set; }
	public Guid HallId { get; set; }
	public Guid SeatTypeId { get; set; }
	public int Row { get; set; }
	public int Column { get; set; }
	public bool Exists { get; set; } = true;


	public SeatModel()
	{
	}
}