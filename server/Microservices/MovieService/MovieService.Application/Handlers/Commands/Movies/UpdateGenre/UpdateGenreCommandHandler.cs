using Mapster;

using MapsterMapper;

using MediatR;

using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Domain.Models;

namespace MovieService.Application.Handlers.Commands.Movies.UpdateGenre;

public class UpdateGenreCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<UpdateGenreCommand, GenreModel>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<GenreModel> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
	{
		var existGenre = await _unitOfWork.Repository<GenreEntity>().GetAsync(request.Id, cancellationToken)
			?? throw new NotFoundException($"Genre with id {request.Id} doesn't exists");

		request.Adapt(existGenre);

		_unitOfWork.Repository<GenreEntity>().Update(existGenre);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return _mapper.Map<GenreModel>(existGenre);
	}
}