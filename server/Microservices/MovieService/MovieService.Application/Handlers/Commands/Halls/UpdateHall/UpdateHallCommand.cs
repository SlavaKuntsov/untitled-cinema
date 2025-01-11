using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Halls.UpdateHall;

public class UpdateHallCommand(
	Guid id,
	string name,
	short totalSeats,
	int[][] seats) : IRequest<HallModel>
{
	public Guid Id { get; private set; } = id;
	public string Name { get; private set; } = name;
	public short TotalSeats { get; private set; } = totalSeats;
	public int[][] Seats { get; private set; } = seats;
}