using MapsterMapper;

using MediatR;
using MovieService.Application.Extensions;
using MovieService.Domain;
using MovieService.Domain.Entities;
using MovieService.Domain.Exceptions;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Application.Handlers.Commands.Movies.CreateMovie;

public class CreateMovieCommandHandler(
	IMoviesRepository moviesRepository,
	IMapper mapper) : IRequestHandler<CreateMovieCommand, Guid>
{
    private readonly IMoviesRepository _moviesRepository = moviesRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<Guid> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        if (!request.ReleaseDate.DateTimeFormatTryParse(out DateTime parsedDateTime))
            throw new BadRequestException("Invalid date format.");

        parsedDateTime = DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Local).ToUniversalTime();

        DateTime dateNow = DateTime.UtcNow;

        var movie = new MovieModel(
            Guid.NewGuid(),
            request.Title,
            request.Genres,
            request.Description,
            request.DurationMinutes,
            request.Producer,
            parsedDateTime,
            dateNow,
            dateNow);

        await _moviesRepository.CreateAsync(_mapper.Map<MovieEntity>(movie), cancellationToken);

        return movie.Id;
    }
}