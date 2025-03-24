using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Database;

public static class DatabaseExtension
{
	public static IServiceCollection AddPostgres<T>(
		this IServiceCollection services,
		IConfiguration configuration) where T : DbContext
	{
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

		var name = typeof(T).Name;

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration.GetConnectionString(name);

		services.AddDbContextPool<T>(options => { options.UseNpgsql(connectionString); },
			128);

		return services;
	}

	public static IServiceCollection AddPostgres<TContextService, TContextImplementation>(
		this IServiceCollection services,
		IConfiguration configuration)
		where TContextImplementation : DbContext, TContextService
		where TContextService : class
	{
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

		var name = typeof(TContextImplementation).Name;

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration.GetConnectionString(name);

		services.AddDbContextPool<TContextService, TContextImplementation>(
			options => { options.UseNpgsql(connectionString); },
			128);

		return services;
	}
}