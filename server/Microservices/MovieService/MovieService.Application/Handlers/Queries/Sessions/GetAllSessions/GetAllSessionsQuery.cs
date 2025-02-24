using MediatR;

using MovieService.Application.DTOs;

namespace MovieService.Application.Handlers.Queries.Sessions.GetAllSessions;

public record GetAllSessionsQuery(
	byte Limit,
	byte Offset,
	Guid? Movie,
	string? Date,
	string? Hall) : IRequest<IList<SessionWithHallDto>>;