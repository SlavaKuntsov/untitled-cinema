using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatById;

public class GetSeatByIdQuery(Guid id) : IRequest<SeatModel>
{
	public Guid Id { get; private set; } = id;
}