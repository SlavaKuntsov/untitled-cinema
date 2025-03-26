using Microsoft.Extensions.Hosting;
using Serilog;

namespace Extensions.Logging;

public static class LoggingExtension
{
	public static IHostBuilder AddLogging(this IHostBuilder hostBuilder)
	{
		hostBuilder.UseSerilog((_, config) =>
		{
			config
				.WriteTo
				.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] {Message}{NewLine}{Exception}");
		});

		return hostBuilder;
	}
}