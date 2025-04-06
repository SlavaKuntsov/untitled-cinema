using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Extensions.Host;

public static class HostExtension
{
	public static WebApplicationBuilder UseHttps(this WebApplicationBuilder builder)
	{
		var environment = builder.Environment;

		var portString = Environment.GetEnvironmentVariable("PORT");

		if (string.IsNullOrEmpty(portString))
			portString = builder.Configuration.GetValue<string>("ApplicationSettings:Port");

		if (!int.TryParse(portString, out var port))
			throw new InvalidOperationException($"Invalid port value: {portString}");

		if (environment.IsProduction())
		{
			const string certPath = "/app/localhost.pfx";
			const string certPassword = "1";

			builder.WebHost.ConfigureKestrel(
				options =>
				{
					options.ListenAnyIP(
						port,
						listenOptions =>
						{
							listenOptions.UseHttps(certPath, certPassword);
						});
				});
		}
		else
		{
			builder.WebHost.ConfigureKestrel(
				options =>
				{
					options.ListenAnyIP(
						port,
						listenOptions =>
						{
							listenOptions.UseHttps();
						});
				});
		}

		return builder;
	}
}