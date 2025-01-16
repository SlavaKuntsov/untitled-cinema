using MapsterMapper;

using MediatR;

using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllGenres;
public class GetAllGenresQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllGenresQuery, IList<GenreModel>>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<IList<GenreModel>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
	{
		var genres = await _unitOfWork.MoviesRepository.GetGenresAsync(cancellationToken);

		return _mapper.Map<IList<GenreModel>>(genres);
	}
}