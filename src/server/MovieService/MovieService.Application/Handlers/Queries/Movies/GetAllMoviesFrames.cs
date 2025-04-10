using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Queries.Movies;

public sealed record GetAllMoviesFramesQuery : IRequest<IList<MovieFrameModel>>;

public sealed class GetAllMoviesFramesQueryHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<GetAllMoviesFramesQuery, IList<MovieFrameModel>>
{
	public async Task<IList<MovieFrameModel>> Handle(GetAllMoviesFramesQuery request, CancellationToken cancellationToken)
	{
		var frames = await unitOfWork.Repository<MovieFrameEntity>()
			.GetAsync(cancellationToken);

		var orderedFrames = frames.OrderBy(f => f.Order);
		
		return mapper.Map<IList<MovieFrameModel>>(orderedFrames);
	}
}