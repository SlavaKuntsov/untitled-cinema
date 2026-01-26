using MediatR;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Seats.UpdateSeat;

public record UpdateSeatCommand(
	Guid Id,
	Guid HallId,
	Guid SeatTypeId,
	int Row,
	int Column) : IRequest<SeatModel>;