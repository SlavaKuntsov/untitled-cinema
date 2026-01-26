using Domain.Exceptions;
using Mapster;
using MapsterMapper;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Movies.UpdateGenre;

public class UpdateGenreCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<UpdateGenreCommand, GenreModel>
{
	public async Task<GenreModel> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
	{
		var existGenre = await unitOfWork.Repository<GenreEntity>().GetAsync(request.Id, cancellationToken)
						?? throw new NotFoundException($"Genre with id {request.Id} doesn't exists");

		request.Adapt(existGenre);

		unitOfWork.Repository<GenreEntity>().Update(existGenre);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return mapper.Map<GenreModel>(existGenre);
	}
}