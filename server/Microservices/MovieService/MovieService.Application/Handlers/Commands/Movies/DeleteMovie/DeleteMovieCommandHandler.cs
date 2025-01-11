using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Movies.DeleteMovie;

public class DeleteMovieCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<DeleteMovieCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
	{
		var movie = await _unitOfWork.MoviesRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Movie with id {request.Id} doesn't exists");

		_unitOfWork.MoviesRepository.Delete(movie);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return;
	}
}