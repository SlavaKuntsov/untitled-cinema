using Domain.Exceptions;
using MediatR;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Movies.DeleteGenre;

public class DeleteGenreCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteGenreCommand>
{
	public async Task Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
	{
		var genre = await unitOfWork.Repository<GenreEntity>().GetAsync(request.Id, cancellationToken)
					?? throw new NotFoundException($"Genre with id {request.Id} doesn't exists");

		unitOfWork.Repository<GenreEntity>().Delete(genre);

		await unitOfWork.SaveChangesAsync(cancellationToken);
	}
}