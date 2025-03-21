using BookingService.Domain.Interfaces.Repositories;
using BookingService.Persistence.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

namespace BookingService.Persistence.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
						   ?? configuration.GetConnectionString("BookingServiceDBContext");

		var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME")
					   ?? configuration["MongoDb:DatabaseName"];

		if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
			throw new InvalidOperationException("Database configuration is missing.");

		services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));

		services.AddScoped(sp =>
		{
			var client = sp.GetRequiredService<IMongoClient>();
			return new BookingServiceDBContext(client, databaseName);
		});

		services.AddScoped<IBookingsRepository, BookingsRepository>();
		services.AddScoped<ISessionSeatsRepository, SessionSeatsRepository>();

		return services;
	}
}