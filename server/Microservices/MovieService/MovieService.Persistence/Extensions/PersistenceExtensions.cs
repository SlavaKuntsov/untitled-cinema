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
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration.GetConnectionString("MovieServiceDBContext");

		services.AddDbContext<MovieServiceDBContext>(options =>
		{
			options.UseNpgsql(connectionString);
		});

		services.AddScoped<IMoviesRepository, MoviesRepository>();
		services.AddScoped<IHallsRepository, HallsRepository>();
		services.AddScoped<IHallSeatsRepository, HallSeatsRepository>();
		services.AddScoped<ISessionsRepository, SessionsRepository>();
		services.AddScoped<IDaysRepository, DaysRepository>();
		services.AddScoped<IMovieGenresRepository, MovieGenresRepository>();

		return services;
	}
}