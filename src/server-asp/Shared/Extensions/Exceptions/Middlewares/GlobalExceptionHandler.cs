using System.ComponentModel.DataAnnotations;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Extensions.Exceptions.Middlewares;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
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
		{ typeof(UnprocessableContentException), (StatusCodes.Status422UnprocessableEntity, "Unprocessable Content") }
	};

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		var (statusCode, title) = ExceptionMappings.TryGetValue(exception.GetType(), out var mapping)
			? mapping
			: (StatusCodes.Status500InternalServerError, "Internal Server Error");

		var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

		httpContext.Response.StatusCode = statusCode;

		return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = new ProblemDetails
			{
				Status = statusCode,
				Title = title,
				Detail = exception.Message,
				Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
				Extensions = new Dictionary<string, object?>
				{
					{ "requestId", httpContext.TraceIdentifier },
					{ "traceId", activity?.Id }
				}
			}
		});
	}
}