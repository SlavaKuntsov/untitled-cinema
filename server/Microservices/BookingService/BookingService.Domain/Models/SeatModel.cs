using System.Text.Json.Serialization;

namespace BookingService.Domain.Models;

public class SeatModel
{
	public Guid Id { get; private set; }
	public int Row { get; private set; }
	public int Column { get; private set; }


	[JsonConstructor]
	public SeatModel(Guid id, int row, int column)
	{
		Id = id;
		Row = row;
		Column = column;
	}
}