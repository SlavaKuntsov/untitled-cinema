namespace MovieService.Domain.Models;

public class HallModel
{
	public Guid Id { get; private set; }
	public string Name { get; set; } = null!;
	public short TotalSeats { get; set; }
	public int[][] SeatsArray { get; set; } = [];

	public HallModel()
	{
	}

	public HallModel(Guid id, string name, short totalSeats, int[][] seats)
	{
		Id = id;
		Name = name;
		TotalSeats = totalSeats;
		SeatsArray = seats;
	}
}