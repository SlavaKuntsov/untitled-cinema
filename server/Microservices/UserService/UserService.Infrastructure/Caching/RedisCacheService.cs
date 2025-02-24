using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Caching.Distributed;

using StackExchange.Redis;

using UserService.Application.Interfaces.Caching;

namespace UserService.Infrastructure.Caching;

public class RedisCacheService : IRedisCacheService
{
	private readonly IDistributedCache _distributedCache;
	private readonly IConnectionMultiplexer _redis;
	private readonly JsonSerializerOptions _serializerOptions;

	public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer redis)
	{
		_distributedCache = distributedCache;
		_redis = redis;

		_serializerOptions = new JsonSerializerOptions
		{
			PropertyNamingPolicy = null,
			IncludeFields = true,
			PropertyNameCaseInsensitive = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};
	}

	public async Task<T> GetValueAsync<T>(string key)
	{
		var data = await _distributedCache.GetStringAsync(key);

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