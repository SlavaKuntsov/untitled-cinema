namespace Redis.Services;

public interface IRedisCacheService
{
	Task<T?> GetValueAsync<T>(string key);
	Task SetValueAsync<T>(string key, T value, TimeSpan? expiry = null);
	Task RemoveValuesByPatternAsync(string pattern);
}