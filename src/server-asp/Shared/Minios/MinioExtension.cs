using Extensions.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minios.Enums;
using Minios.Services;

namespace Minios;

public static class MinioExtension
{
    public static IServiceCollection AddMinio(
       this IServiceCollection services,
       IConfiguration configuration)
    {
       services.Configure<MinioOptions>(
          options =>
             configuration.GetSection(nameof(MinioOptions)).Bind(options));

       services.AddSingleton<IMinioClient>(
          serviceProvider =>
          {
             var options = serviceProvider.GetRequiredService<IOptions<MinioOptions>>().Value
                      ?? throw new ArgumentNullException("MinIO configuration is missing");

             return new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(options.UseSsl)
                .Build();
          });

       services.AddScoped<IMinioService, MinioService>();

       return services;
    }

    public static async Task EnsureMinioBucketExistsAsync(this IServiceProvider serviceProvider)
    {
       using var scope = serviceProvider.CreateScope();

       var client = scope.ServiceProvider.GetRequiredService<IMinioClient>();
       var options = scope.ServiceProvider.GetRequiredService<IOptions<MinioOptions>>().Value;

       var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
       var logger = loggerFactory.CreateLogger("Minio");

       var buckets = new List<string>
       {
          options.DefaultBucket,
          Buckets.Poster.GetDescription(),
          Buckets.Frames.GetDescription()
       };

       foreach (var bucketName in buckets)
       {
          var exists = await client.BucketExistsAsync(
             new BucketExistsArgs().WithBucket(bucketName));

          if (!exists)
          {
             await client.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucketName));

             logger.LogInformation("[MinIO] Bucket '{BucketName}' created.", bucketName);
          }
          else
          {
             logger.LogInformation("[MinIO] Bucket '{BucketName}' already exists.", bucketName);
          }
       }
    }
}