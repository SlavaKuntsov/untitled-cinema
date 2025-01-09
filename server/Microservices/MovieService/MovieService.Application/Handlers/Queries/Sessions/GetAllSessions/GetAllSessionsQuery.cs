using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Sessoins.GetAllSessions;

public partial class GetAllSessionsQuery(
	byte limit,
	byte offset,
	string? date,
	string? hall) : IRequest<IList<SessionModel>>
{
	public byte Limit { get; private set; } = limit;
	public byte Offset { get; private set; } = offset;
	public string? Date { get; private set; } = date;
	public string? Hall { get; private set; } = hall;
}