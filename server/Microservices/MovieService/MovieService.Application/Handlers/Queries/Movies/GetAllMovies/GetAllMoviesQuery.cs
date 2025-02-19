using MediatR;

using MovieService.Application.DTOs;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllMovies;

public record GetAllMoviesQuery(
	byte Limit,
	byte Offset,
	string[] Filters,
	string[] FilterValues,
	string SortBy,
	string SortDirection,
	string Date) : IRequest<PaginationWrapperDto<MovieModel>>;
