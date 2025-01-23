using MediatR;

namespace MovieService.Application.Handlers.Commands.Halls.CreateHall;

public class CreateCustomHallCommand(
	string name,
	short totalSeats,
	int[][] seats) : IRequest<Guid>
{
	public string Name { get; private set; } = name;
	public short TotalSeats { get; private set; } = totalSeats;
	public int[][] Seats { get; private set; } = seats;
}