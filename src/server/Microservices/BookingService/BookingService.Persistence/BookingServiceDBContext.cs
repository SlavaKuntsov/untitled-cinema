using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace BookingService.Persistence;

public class BookingServiceDBContext(IMongoClient client, string databaseName)
{
	private readonly IMongoDatabase _database = client.GetDatabase(databaseName);

	static BookingServiceDBContext()
	{
		var conventions = new ConventionPack
		{
			new CamelCaseElementNameConvention(),
			new IgnoreIfNullConvention(true)
		};

		ConventionRegistry.Register("DefaultConventions", conventions, _ => true);
	}

	public IMongoCollection<T> GetCollection<T>(string name)
	{
		return _database.GetCollection<T>(name);
	}
}