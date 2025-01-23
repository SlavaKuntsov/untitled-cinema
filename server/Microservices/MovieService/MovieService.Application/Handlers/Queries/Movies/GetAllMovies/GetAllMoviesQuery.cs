using MediatR;

using MovieService.Domain;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllMovies;

public class GetAllMoviesQuery(
	byte limit,
	byte offset,
	string? filter,
	string? filterValue,
	string sortBy,
	string sortDirection) : IRequest<IList<MovieModel>>
{
	public byte Limit { get; private set; } = limit;
	public byte Offset { get; private set; } = offset;
	public string? Filter { get; private set; } = filter;
	public string? FilterValue { get; private set; } = filterValue;
	public string SortBy { get; private set; } = sortBy;
	public string SortDirection { get; private set; } = sortDirection;
}