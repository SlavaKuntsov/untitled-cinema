using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

using MovieService.Application.Interfaces.Caching;

namespace MovieService.Infrastructure.Caching;

public class RedisCacheService : IRedisCacheService
{
	private readonly IDistributedCache _distributedCache;

	public RedisCacheService(IDistributedCache distributedCache)
	{
		_distributedCache = distributedCache;
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
}