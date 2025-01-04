using MediatR;

using MovieService.Domain;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllMovies;

public partial class GetAllMoviesQuery() : IRequest<IList<MovieModel>>
{

}