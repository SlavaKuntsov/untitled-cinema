using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces.Repositories;

namespace MovieService.Persistence.Repositories;

public class MovieGenresRepository : IMovieGenresRepository
{
	private readonly MovieServiceDBContext _context;

	public MovieGenresRepository(MovieServiceDBContext context)
	{
		_context = context;
	}

	public async Task<GenreEntity?> GetByNameAsync(string name, CancellationToken cancellationToken)
	{
		return await _context.Genres
			.FirstOrDefaultAsync(g => g.Name == name, cancellationToken);
	}

	public async Task AddAsync(GenreEntity genre, CancellationToken cancellationToken)
	{
		await _context.Genres.AddAsync(genre, cancellationToken);
	}
}