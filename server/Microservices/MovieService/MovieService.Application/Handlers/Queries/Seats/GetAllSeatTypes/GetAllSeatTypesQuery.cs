using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatTypes;

public class GetAllSeatTypesQuery() : IRequest<IList<SeatTypeModel>>
{
}