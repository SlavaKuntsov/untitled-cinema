using MapsterMapper;

using MediatR;

using MovieService.Domain;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Application.Handlers.Queries.Movies.GetMovieById;

public class GetMovieByIdQueryHandler(IMoviesRepository moviesRepository, IMapper mapper) : IRequestHandler<GetMovieByIdQuery, MovieModel?>
{
	private readonly IMoviesRepository _moviesRepository = moviesRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<MovieModel?> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
	{
		var movie = await _moviesRepository.GetAsync(request.Id, cancellationToken);

		return _mapper.Map<MovieModel>(movie);
	}
}