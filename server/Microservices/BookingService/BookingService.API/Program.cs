using BookingService.API.Extensions;
using BookingService.API.Middlewares;
using BookingService.Application.Extensions;
using BookingService.Infrastructure.Extensions;
using BookingService.Persistence.Extensions;

using Brokers.Extensions;

using Hangfire;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;

using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services  = builder.Services;
var configuration = builder.Configuration;

builder.Configuration.AddEnvironmentVariables();

builder.UseHttps();

services.AddAPI(configuration)
	.AddApplication()
	.AddInfrastructure()
	.AddPersistence(configuration)
	.AddRabbitMQ(configuration);

builder.Host.AddLogging(configuration);

var app = builder.Build();

app.UseHangfireDashboard();

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