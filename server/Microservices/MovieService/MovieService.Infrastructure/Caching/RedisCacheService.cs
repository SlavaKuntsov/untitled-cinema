using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

using MovieService.Application.Interfaces.Caching;

using StackExchange.Redis;

namespace MovieService.Infrastructure.Caching;

public class RedisCacheService : IRedisCacheService
{
	private readonly IDistributedCache _distributedCache;
	private readonly IConnectionMultiplexer _redis;

	public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer redis)
	{
		_distributedCache = distributedCache;
		_redis = redis;
	}

	public async Task<T> GetValueAsync<T>(string key)
	{
		var data = await _distributedCache.GetStringAsync(key);

		if (data is null)
			return default(T);

		return JsonSerializer.Deserialize<T>(data)!;
	}

	public async Task SetValueAsync<T>(string key, T value, TimeSpan? expiry = null)
	{
		var options = new DistributedCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = expiry
		};

		var data = JsonSerializer.Serialize(value);

		await _distributedCache.SetStringAsync(key, data, options);
	}

	public async Task RemoveValuesByPatternAsync(string pattern)
	{
		var db = _redis.GetDatabase();
		var server = _redis.GetServer(_redis.GetEndPoints().First());
		var keys = server.Keys(pattern: pattern);

		foreach (var key in keys)
		{
			await db.KeyDeleteAsync(key);
		}
	}
}