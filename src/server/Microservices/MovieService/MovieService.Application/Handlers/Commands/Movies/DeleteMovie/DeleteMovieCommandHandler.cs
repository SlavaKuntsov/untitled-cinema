using Domain.Exceptions;
using MediatR;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using Redis.Service;

namespace MovieService.Application.Handlers.Commands.Movies.DeleteMovie;

public class DeleteMovieCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService) : IRequestHandler<DeleteMovieCommand>
{
	public async Task Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
	{
		var movie = await unitOfWork.MoviesRepository.GetAsync(request.Id, cancellationToken)
					?? throw new NotFoundException($"Movie with id {request.Id} doesn't exists");

		unitOfWork.Repository<MovieEntity>().Delete(movie);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		await redisCacheService.RemoveValuesByPatternAsync("movies_*");
	}
}