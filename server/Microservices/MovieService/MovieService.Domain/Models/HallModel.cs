namespace MovieService.Domain.Models;

public class HallModel
{
	public Guid Id { get; private set; }
	public string Name { get; set; } = null!;
	public short TotalSeats { get; set; }

	public HallModel(Guid id, string name, short totalSeats)
	{
		Id = id;
		Name = name;
		TotalSeats = totalSeats;
	}
}