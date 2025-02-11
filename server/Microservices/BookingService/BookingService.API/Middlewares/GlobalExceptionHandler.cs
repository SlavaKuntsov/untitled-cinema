using System.Diagnostics;

using BookingService.Domain.Exceptions;

using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Middlewares;

public class GlobalExceptionHandler(
	IProblemDetailsService problemDetailsService,
	ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;
	private readonly ILogger<GlobalExceptionHandler> _logger = logger;

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
		{ typeof(UnprocessableContentException), (StatusCodes.Status422UnprocessableEntity, "Unprocessable Content") },
	};

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		var (statusCode, title) = ExceptionMappings
			.TryGetValue(exception.GetType(), out var mapping)
			? mapping
			: (StatusCodes.Status500InternalServerError, "Internal Server Error");

		Activity? activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

		var traceInformation = new Dictionary<string, object?>
		{
			{ "requestId", httpContext.TraceIdentifier },
			{ "traceId", activity?.Id }
		};

		_logger.LogError("An error of type {ExceptionType} occured: {Exception}. " +
			"RequestId {RequestId}. " +
			"TraceId {TraceId}",
			exception.GetType(),
			exception.Message,
			traceInformation["requestId"]!.ToString(),
			traceInformation["traceId"]!.ToString());

		httpContext.Response.StatusCode = statusCode;

		return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = new ProblemDetails
			{
				Status = statusCode,
				Title = title,
				Detail = exception.Message,
				Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
				Extensions = traceInformation
			}
		});
	}
}