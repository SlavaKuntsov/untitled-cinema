using Mapster;

using MapsterMapper;

using MediatR;

using MovieService.Application.Extensions;
using MovieService.Domain;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;

namespace MovieService.Application.Handlers.Commands.Movies.UpdateMovie;

public class UpdateMovieCommandHandler(
	IUnitOfWork unitOfWork,
	IMapper mapper) : IRequestHandler<UpdateMovieCommand, MovieModel>
{
	private readonly IUnitOfWork _unitOfWork = unitOfWork;
	private readonly IMapper _mapper = mapper;

	public async Task<MovieModel> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
	{
		if (!request.ReleaseDate.DateTimeFormatTryParse(out DateTime parsedDateTime))
			throw new BadRequestException("Invalid date format.");

		var existMovie = await _unitOfWork.MoviesRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Movie with id {request.Id} doesn't exists");

		existMovie.UpdatedAt = DateTime.UtcNow;

		request.Adapt(existMovie);

		_unitOfWork.MoviesRepository.Update(existMovie, cancellationToken);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return _mapper.Map<MovieModel>(existMovie);
	}
}