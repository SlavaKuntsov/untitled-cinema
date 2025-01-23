using MediatR;

using MovieService.Domain;

namespace MovieService.Application.Handlers.Queries.Movies.GetMovieById;

public class GetMovieByIdQuery(Guid id) : IRequest<MovieModel>
{
	public Guid Id { get; private set; } = id;
}