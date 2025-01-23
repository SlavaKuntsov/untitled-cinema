namespace MovieService.Domain.Models;

public class SeatModel
{
	public Guid Id { get; set; }
	public Guid HallId { get; set; }
	public Guid SeatTypeId { get; set; }
	public int Row { get; set; }
	public int Column { get; set; }

	public SeatModel()
	{
	}

	public SeatModel(
		Guid id,
		Guid hallId,
		Guid seatTypeId,
		int row,
		int column)
	{
		Id = id;
		HallId = hallId;
		SeatTypeId = seatTypeId;
		Row = row;
		Column = column;
	}
}