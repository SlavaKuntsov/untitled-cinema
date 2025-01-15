using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetAllSeatById;

public class GetSeatsBySessionIdQuery(Guid id) : IRequest<IList<SeatModel>>
{
	public Guid Id { get; private set; } = id;
}