using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Movies.DeleteGenre;

public class DeleteGenreCommandHandler(
	IUnitOfWork unitOfWork) : IRequestHandler<DeleteGenreCommand>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;

	public async Task Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
	{
		var genre = await _unitOfWork.Repository<GenreEntity>().GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Genre with id {request.Id} doesn't exists");

		_unitOfWork.Repository<GenreEntity>().Delete(genre);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return;
	}
}