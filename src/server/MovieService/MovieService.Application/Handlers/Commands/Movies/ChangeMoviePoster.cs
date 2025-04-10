using Domain.Exceptions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Minios.Services;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using Redis.Services;

namespace MovieService.Application.Handlers.Commands.Movies;

public sealed record ChangeMoviePosterCommand(
	Guid Id,
	IFormFile Poster) : IRequest<string>;

public sealed class ChangeMoviePosterCommandHandler(
	IUnitOfWork unitOfWork,
	IRedisCacheService redisCacheService,
	IMinioService minioService) : IRequestHandler<ChangeMoviePosterCommand, string>
{
	public async Task<string> Handle(
		ChangeMoviePosterCommand request,
		CancellationToken cancellationToken)
	{
		var existMovie = await unitOfWork.MoviesRepository.GetAsync(request.Id, cancellationToken)
						?? throw new NotFoundException($"Movie with id '{request.Id}' doesn't exists");

		var posterName = minioService.CreateObjectName(request.Poster.FileName);

		await using var stream = request.Poster.OpenReadStream();

		await minioService.UploadFileAsync(
			null,
			posterName,
			stream,
			request.Poster.ContentType);

		existMovie.Poster = posterName;

		unitOfWork.Repository<MovieEntity>().Update(existMovie);

		await unitOfWork.SaveChangesAsync(cancellationToken);
		
		await redisCacheService.RemoveValuesByPatternAsync("movies_*");

		return posterName;
	}
}