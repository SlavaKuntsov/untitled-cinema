using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Movies.GetAllGenres;

public class GetAllGenresQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllGenresQuery, IList<GenreModel>>
{
	public async Task<IList<GenreModel>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
	{
		var genres = await unitOfWork.Repository<GenreEntity>().GetAsync(cancellationToken);

		return mapper.Map<IList<GenreModel>>(genres);
	}
}