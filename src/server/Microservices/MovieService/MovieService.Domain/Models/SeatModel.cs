namespace MovieService.Domain.Models;

public class SeatModel
{
	public Guid Id { get; private set; }
	public Guid HallId { get; private set; }
	public Guid SeatTypeId { get; private set; }
	public int Row { get; private set; }
	public int Column { get; private set; }

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