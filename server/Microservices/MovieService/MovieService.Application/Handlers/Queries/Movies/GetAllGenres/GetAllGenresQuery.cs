using MediatR;

using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllGenres;

public class GetAllGenresQuery() : IRequest<IList<GenreModel>>
{
}