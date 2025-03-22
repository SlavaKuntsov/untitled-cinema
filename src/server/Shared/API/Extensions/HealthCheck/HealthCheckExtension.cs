using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.HealthCheck;

public static class HealthCheckExtension
{
	public static WebApplication AddHealthCheck(
		this WebApplication app,
		IServiceCollection services)
	{
		services.AddHealthChecks();

		app.MapHealthChecks(
			"/health",
			new HealthCheckOptions
			{
				ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
			});

		return app;
	}
}