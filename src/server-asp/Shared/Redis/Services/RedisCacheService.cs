using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Redis.Services;

public class RedisCacheService : IRedisCacheService
{
	private static bool? _redisAvailabilityChecked;

	private readonly IDistributedCache? _distributedCache;

	private readonly bool _isAvailable;

	private readonly ILogger<RedisCacheService> _logger;

	private readonly IConnectionMultiplexer? _redis;

	private readonly JsonSerializerOptions _serializerOptions = new()
	{
		PropertyNamingPolicy = null,
		IncludeFields = true,
		PropertyNameCaseInsensitive = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public RedisCacheService(
		IDistributedCache? distributedCache,
		IConnectionMultiplexer? redis,
		ILogger<RedisCacheService> logger,
		bool isAvailable)
	{
		_distributedCache = distributedCache;
		_redis = redis;
		_logger = logger;
		_isAvailable = isAvailable;

		// При первом создании сервиса устанавливаем флаг доступности
		if (_redisAvailabilityChecked == null)
		{
			_redisAvailabilityChecked = isAvailable;

			if (!isAvailable)
				_logger.LogWarning(
					"Redis has been marked as unavailable. Caching will be disabled for all requests.");
		}
	}

	public async Task<T?> GetValueAsync<T>(string key)
	{
		// Используем статическую переменную для проверки доступности
		if (_redisAvailabilityChecked == false)
		{
			_logger.LogDebug("Redis unavailable: GetValueAsync for key {Key} skipped", key);

			return default;
		}

		try
		{
			var data = await _distributedCache!.GetStringAsync(key);

			if (data is null)
				return default;

			return JsonSerializer.Deserialize<T>(data, _serializerOptions);
		}
		catch (Exception ex)
		{
			// Если произошла ошибка, помечаем Redis как недоступный
			if (_redisAvailabilityChecked == true)
			{
				_redisAvailabilityChecked = false;

				_logger.LogWarning(
					ex,
					"Redis connection failed. Caching will be disabled for all subsequent requests.");
			}

			return default;
		}
	}

	public async Task SetValueAsync<T>(string key, T value, TimeSpan? expiry = null)
	{
		if (_redisAvailabilityChecked == false)
		{
			_logger.LogDebug("Redis unavailable: SetValueAsync for key {Key} skipped", key);

			return;
		}

		try
		{
			var options = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = expiry,
				SlidingExpiration = TimeSpan.FromMinutes(5)
			};

			var data = JsonSerializer.Serialize(value, _serializerOptions);

			await _distributedCache!.SetStringAsync(key, data, options);
		}
		catch (Exception ex)
		{
			if (_redisAvailabilityChecked == true)
			{
				_redisAvailabilityChecked = false;

				_logger.LogWarning(
					ex,
					"Redis connection failed. Caching will be disabled for all subsequent requests.");
			}
		}
	}

	public async Task RemoveValuesByPatternAsync(string pattern)
	{
		if (_redisAvailabilityChecked == false)
		{
			_logger.LogDebug(
				"Redis unavailable: RemoveValuesByPatternAsync for pattern {Pattern} skipped",
				pattern);

			return;
		}

		try
		{
			var db = _redis!.GetDatabase();
			var server = _redis.GetServer(_redis.GetEndPoints().First());
			var keys = server.Keys(pattern: pattern);

			foreach (var key in keys)
				await db.KeyDeleteAsync(key);
		}
		catch (Exception ex)
		{
			if (_redisAvailabilityChecked == true)
			{
				_redisAvailabilityChecked = false;

				_logger.LogWarning(
					ex,
					"Redis connection failed. Caching will be disabled for all subsequent requests.");
			}
		}
	}
}