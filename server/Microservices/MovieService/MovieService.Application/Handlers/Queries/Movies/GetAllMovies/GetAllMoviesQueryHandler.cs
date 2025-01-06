using MapsterMapper;

using MediatR;

using MovieService.Domain;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllMovies;

public class GetAllMoviesQueryHandler(
	IMoviesRepository moviesRepository,
	IMapper mapper) : IRequestHandler<GetAllMoviesQuery, IList<MovieModel>>
{
	private readonly IMoviesRepository _moviesRepository = moviesRepository;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<MovieModel>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
	{
		var movies = await _moviesRepository.GetAsync(cancellationToken);

		return _mapper.Map<IList<MovieModel>>(movies);
	}
}