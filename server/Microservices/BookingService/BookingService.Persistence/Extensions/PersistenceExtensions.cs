using BookingService.Domain.Interfaces.Repositories;
using BookingService.Persistence.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookingService.Persistence.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
		var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME");

		if (string.IsNullOrEmpty(connectionString))
		{
			connectionString = configuration.GetConnectionString("BookingServiceDBContext");
			databaseName = configuration["MongoDb:DatabaseName"];
		}

		services.AddSingleton(sp =>
		{
			return new BookingServiceDBContext(connectionString!, databaseName!);
		});

		services.AddScoped<IBookingsRepository, BookingsRepository>();
		services.AddScoped<ISessionSeatsRepository, SessionSeatsRepository>();

		return services;
	}
}