using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Extensions.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(ILogger logger)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : class
	where TResponse : IResult<TResponse>
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var requestName = typeof(TRequest).Name;

		logger.LogInformation("Processing request {RequestName}", requestName);

		var result = await next();

		if (result.IsSuccess)
			logger.LogInformation("Completed request {RequestName}", requestName);
		else
			using (LogContext.PushProperty("Error", result.Error, true))
			{
				logger.LogError("Completed request {RequestName} with error", requestName);
			}

		return result;
	}
}