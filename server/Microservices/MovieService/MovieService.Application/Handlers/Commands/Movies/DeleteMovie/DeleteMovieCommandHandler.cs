using MediatR;

using MovieService.Application.Interfaces.Caching;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Movies.DeleteMovie;

public class DeleteMovieCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService) : IRequestHandler<DeleteMovieCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IRedisCacheService _redisCacheService = redisCacheService;

	public async Task Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
	{
		var movie = await _unitOfWork.MoviesRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Movie with id {request.Id} doesn't exists");

		_unitOfWork.Repository<MovieEntity>().Delete(movie);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		await _redisCacheService.RemoveValuesByPatternAsync("movies_*");

		return;
	}
}