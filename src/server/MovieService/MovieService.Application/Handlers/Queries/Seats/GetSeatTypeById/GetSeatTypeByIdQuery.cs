using MediatR;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatTypeById;

public class GetSeatTypeByIdQuery(Guid id) : IRequest<SeatTypeModel>
{
	public Guid Id { get; private set; } = id;
}