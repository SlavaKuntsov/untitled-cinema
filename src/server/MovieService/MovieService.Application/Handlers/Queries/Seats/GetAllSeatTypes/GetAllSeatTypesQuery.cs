using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetAllSeatTypes;

public class GetAllSeatTypesQuery() : IRequest<IList<SeatTypeModel>>
{
}