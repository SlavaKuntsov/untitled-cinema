using Domain.Exceptions;
using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Movies.GetMovieById;

public class GetMovieByIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetMovieByIdQuery, MovieModel?>
{
	public async Task<MovieModel?> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
	{
		var movie = await unitOfWork.MoviesRepository.GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Movie with id '{request.ToString()}' not found.");

		return mapper.Map<MovieModel>(movie);
	}
}