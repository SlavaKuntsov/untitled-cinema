﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MovieService.Domain.Interfaces.Repositories;
using MovieService.Domain.Interfaces.Repositories.UnitOfWork;
using MovieService.Persistence.Repositories;
using MovieService.Persistence.Repositories.UnitOfWork;

namespace MovieService.Persistence.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration.GetConnectionString("MovieServiceDBContext");

		services.AddDbContextPool<MovieServiceDBContext>(options =>
		{
			options.UseNpgsql(connectionString);
		}, poolSize: 128);

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