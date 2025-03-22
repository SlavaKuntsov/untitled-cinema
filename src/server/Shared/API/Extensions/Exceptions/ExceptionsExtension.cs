using Extensions.Exceptions.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using UserService.API.Behaviors;
using UserService.API.Middlewares;

namespace Extensions.Exceptions;

public static class ExceptionsExtension
{
	public static WebApplication AddExceptions(
		this WebApplication app,
		IServiceCollection services)
	{
		services.AddExceptionHandler<GlobalExceptionHandler>();

		app.UseExceptionHandler();

		app.UseMiddleware<RequestLogContextMiddleware>();

		return app;
	}
}