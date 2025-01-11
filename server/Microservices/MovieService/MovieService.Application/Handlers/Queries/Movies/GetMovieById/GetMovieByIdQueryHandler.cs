using MapsterMapper;

using MediatR;

using MovieService.Domain;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Queries.Movies.GetMovieById;

public class GetMovieByIdQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetMovieByIdQuery, MovieModel?>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<MovieModel?> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
	{
		var movie = await _unitOfWork.MoviesRepository.GetAsync(request.Id, cancellationToken);

		return _mapper.Map<MovieModel>(movie);
	}
}