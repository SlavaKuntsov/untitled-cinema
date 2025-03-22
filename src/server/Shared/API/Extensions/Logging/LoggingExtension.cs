using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Extensions.Logging;

public static class LoggingExtension
{
	public static WebApplication AddLogging(
		this WebApplication app,
		IHostBuilder hostBuilder)
	{
		app.UseSerilogRequestLogging();
		
		hostBuilder.UseSerilog((_, config) =>
		{
			config
				.WriteTo
				.Console(outputTemplate: "{Timestamp:HH:mm} [{Level}] {Message}{NewLine}{Exception}");
		});

		return app;
	}
}