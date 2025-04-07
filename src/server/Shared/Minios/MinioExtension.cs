using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace Minios;

public static class MinioExtension
{
	public static IServiceCollection AddMinio(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		// Регистрируем конфигурацию (исправленная строка)
		services.Configure<MinioOptions>(options => 
			configuration.GetSection(nameof(MinioOptions)).Bind(options));

		// Регистрируем MinioClient как Singleton
		services.AddSingleton<IMinioClient>(serviceProvider =>
		{
			var options = serviceProvider.GetRequiredService<IOptions<MinioOptions>>().Value
						?? throw new ArgumentNullException("Minio configuration is missing");

			return new MinioClient()
				.WithEndpoint(options.Endpoint)
				.WithCredentials(options.AccessKey, options.SecretKey)
				.WithSSL(options.UseSsl)
				.Build();
		});

		return services;
	}
}