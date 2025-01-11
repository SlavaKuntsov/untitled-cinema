namespace MovieService.Domain.Models;

public class HallSeatModel
{
	public Guid Id { get; set; }
	public Guid HallId { get; set; }
	public int SeatRow { get; set; }
	public int SeatColumn { get; set; }
	public bool Exists { get; set; } = true;
	public string SeatType { get; set; } = string.Empty;
	public decimal Price { get; set; } = 0.00m;

	public HallSeatModel()
	{
	}
}