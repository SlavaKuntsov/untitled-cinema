using Domain.Exceptions;
using Extensions.Strings;
using Mapster;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;
using Redis.Service;

namespace MovieService.Application.Handlers.Commands.Movies.UpdateMovie;

public class UpdateMovieCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService,
	IMapper mapper) : IRequestHandler<UpdateMovieCommand, MovieModel>
{
	public async Task<MovieModel> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
	{
		if (!request.ReleaseDate.DateTimeFormatTryParse(out _))
			throw new BadRequestException("Invalid date format.");

		var existMovie = await unitOfWork.MoviesRepository.GetAsync(request.Id, cancellationToken)
						?? throw new NotFoundException($"Movie with id '{request.Id}' doesn't exists");

		existMovie.UpdatedAt = DateTime.UtcNow;

		request.Adapt(existMovie);

		unitOfWork.Repository<MovieEntity>().Update(existMovie);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		await redisCacheService.RemoveValuesByPatternAsync("movies_*");

		return mapper.Map<MovieModel>(existMovie);
	}
}