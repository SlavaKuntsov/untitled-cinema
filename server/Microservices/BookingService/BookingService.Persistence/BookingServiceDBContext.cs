using Microsoft.EntityFrameworkCore;

using MongoDB.Bson.Serialization.Conventions;

using MongoDB.Driver;

namespace BookingService.Persistence;

public class BookingServiceDBContext : DbContext
{
	private readonly IMongoDatabase _database;

	static BookingServiceDBContext()
	{
		var conventions = new ConventionPack
			{
				new CamelCaseElementNameConvention(),
				new IgnoreIfNullConvention(true),
			};

		ConventionRegistry.Register("DefaultConvetions", conventions, type => true);
	}

	public BookingServiceDBContext(string connectionString, string databaseName)
	{
		var client = new MongoClient(connectionString);
		_database = client.GetDatabase(databaseName);
	}

	public IMongoCollection<T> GetCollection<T>(string name)
	{
		return _database.GetCollection<T>(name);
	}
}