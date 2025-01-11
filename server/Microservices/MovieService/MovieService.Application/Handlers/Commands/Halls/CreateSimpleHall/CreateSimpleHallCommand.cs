using MediatR;

namespace MovieService.Application.Handlers.Commands.Halls.CreateSimpleHall;

public class CreateSimpleHallCommand(
	string name,
	short totalSeats,
	byte rows,
	byte columns) : IRequest<Guid>
{
	public string Name { get; private set; } = name;
	public short TotalSeats { get; private set; } = totalSeats;
	public byte Rows { get; private set; } = rows;
	public byte Columns { get; private set; } = columns;
}