using Brokers.Extensions;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;

using MovieService.API.Extensions;
using MovieService.API.Middlewares;
using MovieService.Application.Extensions;
using MovieService.Infrastructure.Extensions;
using MovieService.Persistence.Extensions;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services  = builder.Services;
var configuration = builder.Configuration;

builder.Configuration.AddEnvironmentVariables();

builder.UseHttps();

services.AddAPI(configuration)
	.AddApplication()
	.AddInfrastructure(configuration)
	.AddPersistence(configuration)
	.AddRabbitMQ(configuration);

builder.Host.AddLogging(configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks(
	"/health",
	new HealthCheckOptions
	{
		ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
	});

app.UseCookiePolicy(new CookiePolicyOptions
{
	MinimumSameSitePolicy = SameSiteMode.None,
	HttpOnly = HttpOnlyPolicy.Always,
	Secure = CookieSecurePolicy.Always
});
app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.All
});
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();