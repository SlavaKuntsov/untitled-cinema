using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using UserService.Domain.Exceptions;

namespace UserService.API.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
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

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
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

		httpContext.Response.ContentType = "application/json";
		httpContext.Response.StatusCode = statusCode;

		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

		return true;
	}
}