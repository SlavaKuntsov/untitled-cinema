using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MovieService.Domain.Interfaces.Repositories;
using MovieService.Persistence.Repositories;

namespace MovieService.Persistence.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<MovieServiceDBContext>(options =>
		{
			options.UseNpgsql(configuration.GetConnectionString(nameof(MovieServiceDBContext)));
		});

		services.AddScoped<IMoviesRepository, MoviesRepository>();

		return services;
	}
}