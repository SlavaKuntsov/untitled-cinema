using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Seats.UpdateSeat;

public class UpdateSeatCommand(
	Guid id,
	Guid hallId,
	Guid seatTypeId,
	int row,
	int column) : IRequest<SeatModel>
{
	public Guid Id { get; set; } = id;
	public Guid HallId { get; set; } = hallId;
	public Guid SeatTypeId { get; set; } = seatTypeId;
	public int Row { get; set; } = row;
	public int Column { get; set; } = column;
}