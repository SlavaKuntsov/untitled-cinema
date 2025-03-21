using MediatR;

namespace MovieService.Application.Handlers.Commands.Seats.CreateSeat;

public class CreateSeatCommand(
	string hall,
	string seatType,
	int row,
	int column) : IRequest<Guid>
{
	public string Hall { get; private set; } = hall;
	public string SeatType { get; private set; } = seatType;
	public int Row { get; private set; } = row;
	public int Column { get; private set; } = column;
}