using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Movies.DeleteGenre;

public class DeleteGenreCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<DeleteGenreCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
	{
		var genre = await _unitOfWork.MoviesRepository.GetGenreAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Genre with id {request.Id} doesn't exists");

		_unitOfWork.MoviesRepository.Delete(genre);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return;
	}
}