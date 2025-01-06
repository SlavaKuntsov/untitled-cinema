using MapsterMapper;

using MediatR;

using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Application.Handlers.Commands.Movies.DeleteMovie;

public class DeleteMovieCommandHandler(
	IMoviesRepository moviesRepository,
	IMapper mapper) : IRequestHandler<DeleteMovieCommand>
{
	private readonly IMoviesRepository _moviesRepository = moviesRepository;
	private readonly IMapper _mapper = mapper;

	public async Task Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
	{
		var movie = await _moviesRepository.GetAsync(request.Id, cancellationToken)
				?? throw new NotFoundException($"Movie with id {request.Id} doesn't exists");

		_moviesRepository.Delete(movie);

		return;
	}
}