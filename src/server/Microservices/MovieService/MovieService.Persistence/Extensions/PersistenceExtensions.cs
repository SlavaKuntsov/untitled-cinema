using Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Persistence.Repositories;
using MovieService.Persistence.Repositories.UnitOfWork;

namespace MovieService.Persistence.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddPostgres<MovieServiceDBContext>(configuration);
		
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		
		services.AddScoped<IMoviesRepository, MoviesRepository>();
		services.AddScoped<IHallsRepository, HallsRepository>();
		services.AddScoped<ISeatsRepository, SeatsRepository>();
		services.AddScoped<ISessionsRepository, SessionsRepository>();
		services.AddScoped<IDaysRepository, DaysRepository>();
		services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

		return services;
	}
}