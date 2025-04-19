using Domain.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Minios.Services;
using MovieService.Domain.Entities.Movies;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Movies;

public sealed record AddMovieFrameCommand(
	Guid MovieId,
	int Order,
	IFormFile Frame) : IRequest<string>;

public sealed class AddMovieFrameCommandHandler(
	IUnitOfWork unitOfWork,
	IMinioService minioService,
	IMapper mapper) : IRequestHandler<AddMovieFrameCommand, string>
{
	public async Task<string> Handle(AddMovieFrameCommand request, CancellationToken cancellationToken)
	{
		_ = await unitOfWork.Repository<MovieEntity>()
				.GetAsync(request.MovieId, cancellationToken)
			?? throw new NotFoundException($"Movie with id {request.MovieId} not found.");

		var existFrames = await unitOfWork.MoviesRepository.GetFramesAsync(
			request.MovieId,
			cancellationToken);

		if (request.Order != -1)
			if (existFrames.Any(f => f.Order == request.Order))
			{
				var framesToUpdate = existFrames
					.Where(f => f.Order >= request.Order)
					.OrderBy(f => f.Order)
					.ToList();

				foreach (var frame in framesToUpdate)
				{
					frame.Order += 1;
					unitOfWork.Repository<MovieFrameEntity>().Update(frame);
				}
			}

		var frameName = minioService.CreateObjectName(request.Frame.FileName);

		var frameModel = new MovieFrameModel(
			Guid.NewGuid(),
			request.MovieId,
			frameName,
			request.Order == -1 ? existFrames.Count : request.Order);

		await using var stream = request.Frame.OpenReadStream();

		await minioService.UploadFileAsync(
			null,
			frameName,
			stream,
			request.Frame.ContentType);

		var frameEntity = mapper.Map<MovieFrameEntity>(frameModel);

		await unitOfWork.Repository<MovieFrameEntity>().CreateAsync(frameEntity, cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return frameName;
	}
}