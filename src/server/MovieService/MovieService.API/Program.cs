using Brokers;
using Extensions.Authorization;
using Extensions.Common;
using Extensions.Exceptions;
using Extensions.Exceptions.Middlewares;
using Extensions.Host;
using Extensions.Logging;
using Extensions.Swagger;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Minios;
using MovieService.API.Extensions;
using MovieService.Application.Extensions;
using MovieService.Infrastructure.Extensions;
using MovieService.Persistence.Extensions;
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
	// .AddMapper()
	.AddSwagger()
	.AddRabbitMQ(configuration)
	.AddMinio(configuration)
	.AddHealthChecks();

services
	.AddAPI(configuration)
	.AddApplication()
	.AddInfrastructure(configuration)
	.AddPersistence(configuration);

host.AddLogging();

var app = builder.Build();

if (app.Environment.IsProduction())
{
	using var scope = app.Services.CreateScope();
	await scope.ServiceProvider.EnsureMinioBucketExistsAsync();
}

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