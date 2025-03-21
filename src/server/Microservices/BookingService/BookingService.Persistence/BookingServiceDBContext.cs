using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace BookingService.Persistence;

public class BookingServiceDBContext
{
	private readonly IMongoDatabase _database;

	static BookingServiceDBContext()
	{
		var conventions = new ConventionPack
		{
			new CamelCaseElementNameConvention(),
			new IgnoreIfNullConvention(true),
		};

		ConventionRegistry.Register("DefaultConventions", conventions, _ => true);
	}

	public BookingServiceDBContext(IMongoClient client, string databaseName)
	{
		_database = client.GetDatabase(databaseName);
	}

	public IMongoCollection<T> GetCollection<T>(string name)
	{
		return _database.GetCollection<T>(name);
	}
}