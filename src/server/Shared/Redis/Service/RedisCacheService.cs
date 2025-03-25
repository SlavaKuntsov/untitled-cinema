using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Redis.Service;

public class RedisCacheService(
	IDistributedCache distributedCache,
	IConnectionMultiplexer redis)
	: IRedisCacheService
{
	private readonly JsonSerializerOptions _serializerOptions = new()
	{
		PropertyNamingPolicy = null,
		IncludeFields = true,
		PropertyNameCaseInsensitive = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public async Task<T> GetValueAsync<T>(string key)
	{
		var data = await distributedCache.GetStringAsync(key);

		if (data is null)
			return default;

		return JsonSerializer.Deserialize<T>(data, _serializerOptions)!;
	}

	public async Task SetValueAsync<T>(string key, T value, TimeSpan? expiry = null)
	{
		var options = new DistributedCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = expiry,
			SlidingExpiration = TimeSpan.FromMinutes(5)
		};

		var data = JsonSerializer.Serialize(value, _serializerOptions);

		await distributedCache.SetStringAsync(key, data, options);
	}

	public async Task RemoveValuesByPatternAsync(string pattern)
	{
		var db = redis.GetDatabase();
		var server = redis.GetServer(redis.GetEndPoints().First());
		var keys = server.Keys(pattern: pattern);

		foreach (var key in keys)
			await db.KeyDeleteAsync(key);
	}
}