using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

namespace BookingService.Persistence.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration.GetConnectionString("BookingServiceDBContext");

		services.AddSingleton<IMongoClient, MongoClient>(sp =>
		{
			return new MongoClient(connectionString);
		});

		services.AddScoped(sp =>
		{
			var client = sp.GetRequiredService<IMongoClient>();
			return client.GetDatabase(configuration["MongoDb:DatabaseName"]);
		});

		return services;
	}
}