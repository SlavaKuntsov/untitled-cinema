using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Redis.Services;
using StackExchange.Redis;

namespace Redis;

public static class RedisExtension
{
    public static IServiceCollection AddRedis(
       this IServiceCollection services,
       IConfiguration configuration)
    {
       services.AddSingleton<IRedisCacheService, RedisCacheService>(
          serviceProvider =>
          {
             var logger = serviceProvider.GetRequiredService<ILogger<RedisCacheService>>();
             var connectionString = Environment.GetEnvironmentVariable("REDIS_CONFIGURATION");

             if (string.IsNullOrEmpty(connectionString))
                connectionString = configuration.GetConnectionString("Redis");

             try
             {
                // Try to setup Redis
                var configOptions = ConfigurationOptions.Parse(connectionString, true);
                configOptions.AbortOnConnectFail = false;
                configOptions.ConnectTimeout = 1000; // Уменьшаем таймаут до 1 секунды
                configOptions.SyncTimeout = 1000;
                configOptions.ConnectRetry = 1; // Только одна попытка подключения
                
                var multiplexer = ConnectionMultiplexer.Connect(configOptions);
                
                // Сразу проверяем, действительно ли подключение работает
                var db = multiplexer.GetDatabase();
                var pingResult = db.Ping(); // Это даст ошибку, если Redis недоступен
                
                var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();

                logger.LogInformation("Successfully connected to Redis");
                return new RedisCacheService(distributedCache, multiplexer, logger, true);
             }
             catch (Exception ex)
             {
                logger.LogWarning(ex, "Redis connection failed. Caching will be disabled.");
                return new RedisCacheService(null, null, logger, false);
             }
          });

       try
       {
          var connectionString = Environment.GetEnvironmentVariable("REDIS_CONFIGURATION");

          if (string.IsNullOrEmpty(connectionString))
             connectionString = configuration.GetConnectionString("Redis");

          var configOptions = ConfigurationOptions.Parse(connectionString, true);
          configOptions.AbortOnConnectFail = false;
          configOptions.ConnectTimeout = 1000; // Уменьшаем таймаут до 1 секунды
          configOptions.SyncTimeout = 1000;
          configOptions.ConnectRetry = 1; // Только одна попытка подключения
          
          services.AddStackExchangeRedisCache(
             options =>
             {
                options.Configuration = connectionString;
                options.InstanceName = string.Empty;
                options.ConfigurationOptions = configOptions;
             });

          services.AddSingleton<IConnectionMultiplexer>(
             _ => ConnectionMultiplexer.Connect(configOptions));
       }
       catch (Exception ex)
       {
          // If Redis setup fails, register a memory cache as fallback
          services.AddDistributedMemoryCache();
       }

       return services;
    }
}