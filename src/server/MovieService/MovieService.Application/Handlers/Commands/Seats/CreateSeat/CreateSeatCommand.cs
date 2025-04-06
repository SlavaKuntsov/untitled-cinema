using MediatR;

namespace MovieService.Application.Handlers.Commands.Seats.CreateSeat;

public class CreateSeatCommand(
	Guid hallId,
	string seatType,
	int row,
	int column) : IRequest<Guid>
{
	public Guid HallId { get; private set; } = hallId;
	public string SeatType { get; private set; } = seatType;
	public int Row { get; private set; } = row;
	public int Column { get; private set; } = column;
}