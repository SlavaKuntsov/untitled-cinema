using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Halls.GetHallById;

public partial class GetHallByIdQuery(Guid id) : IRequest<HallModel>
{
	public Guid Id { get; private set; } = id;
}