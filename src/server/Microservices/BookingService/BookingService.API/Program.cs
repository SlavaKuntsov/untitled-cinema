using BookingService.API.Extensions;
using BookingService.Application.Extensions;
using BookingService.Infrastructure.Extensions;
using BookingService.Persistence.Extensions;
using Brokers;
using Extensions.Authorization;
using Extensions.Common;
using Extensions.Exceptions;
using Extensions.Exceptions.Middlewares;
using Extensions.Host;
using Extensions.Logging;
using Extensions.Mapper;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var host = builder.Host;

builder.UseHttps();

services
	.AddCommon()
	.AddExceptions()
	.AddAuthorization(configuration)
	.AddMapper()
	.AddRabbitMQ(configuration)
	.AddHealthChecks();

services
	.AddAPI(configuration)
	.AddApplication()
	.AddInfrastructure()
	.AddPersistence(configuration);

host.AddLogging();

var app = builder.Build();

app.AddAPI();

app.UseHangfireDashboard();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseExceptionHandler();
app.UseMiddleware<RequestLogContextMiddleware>();

app.UseSerilogRequestLogging();

app.MapHealthChecks(
	"/health",
	new HealthCheckOptions
	{
		ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
	});

app.UseCookiePolicy(
	new CookiePolicyOptions
	{
		MinimumSameSitePolicy = SameSiteMode.None,
		HttpOnly = HttpOnlyPolicy.Always,
		Secure = CookieSecurePolicy.Always
	});

app.UseForwardedHeaders(
	new ForwardedHeadersOptions
	{
		ForwardedHeaders = ForwardedHeaders.All
	});
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();