using System.Text.Json;

using FluentValidation;

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

	private static readonly Dictionary<Type, (int StatusCode, string Title)> ExceptionMappings = new()
	{
		{ typeof(AlreadyExistsException), (StatusCodes.Status400BadRequest, "Resource Already Exists") },
		{ typeof(BadRequestException), (StatusCodes.Status400BadRequest, "Bad Request") },
		{ typeof(NotFoundException), (StatusCodes.Status404NotFound, "Resource Not Found") },
		{ typeof(ValidationProblemException), (StatusCodes.Status400BadRequest, "Validation Error") },
		{ typeof(UnauthorizedAccessException), (StatusCodes.Status401Unauthorized, "Unauthorized") },
		{ typeof(InvalidTokenException), (StatusCodes.Status400BadRequest, "Invalid Token") },
		{ typeof(ValidationException), (StatusCodes.Status400BadRequest, "Invalid Data") },
		{ typeof(InvalidOperationException), (StatusCodes.Status400BadRequest, "Invalid Operation") },
	};

	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		var (statusCode, title) = ExceptionMappings.TryGetValue(exception.GetType(), out var mapping)
			? mapping
			: (StatusCodes.Status500InternalServerError, "Internal Server Error");

		var problemDetails = new ProblemDetails
		{
			Status = statusCode,
			Title = title,
			Detail = exception.Message
		};

		context.Response.ContentType = "application/json";
		context.Response.StatusCode = statusCode;

		var response = JsonSerializer.Serialize(problemDetails);
		return context.Response.WriteAsync(response);
	}
}