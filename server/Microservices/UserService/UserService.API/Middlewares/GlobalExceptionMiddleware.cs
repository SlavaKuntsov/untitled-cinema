using System.Net;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using UserService.Domain.Exceptions;

namespace UserService.API.Middlewares;

public class GlobalExceptionMiddleware
{
	private readonly RequestDelegate _next;

	public GlobalExceptionMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception exception)
		{
			await HandleExceptionAsync(context, exception);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		var statusCode = exception switch
		{
			ValidationProblemException => HttpStatusCode.BadRequest,
			BadRequestException => HttpStatusCode.BadRequest,
			NotFoundException => HttpStatusCode.NotFound,
			AlreadyExistsException => HttpStatusCode.Conflict,
			NotImplementedException => HttpStatusCode.NotImplemented,
			UnauthorizedAccessException => HttpStatusCode.Unauthorized,
			_ => HttpStatusCode.InternalServerError,
		};

		var problemDetails = new ProblemDetails
		{
			Status = (int)statusCode,
			Title = "An error occurred",
			Detail = exception.Message
		};

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)statusCode;

		var response = JsonSerializer.Serialize(problemDetails);
		return context.Response.WriteAsync(response);
	}
}
