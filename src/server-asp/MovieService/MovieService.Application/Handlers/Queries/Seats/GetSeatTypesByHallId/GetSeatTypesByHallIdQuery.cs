using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Seats.GetSeatTypesByHallId;

public record GetSeatTypesByHallIdQuery(Guid HallId) : IRequest<IList<SeatTypeModel>>;