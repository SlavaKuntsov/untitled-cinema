using CSharpFunctionalExtensions;

using MediatR;

using Serilog.Context;

namespace BookingService.API.Behaviors;

internal sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : class
	where TResponse : IResult<TResponse>
{
	private readonly ILogger _logger;

	public RequestLoggingPipelineBehavior(ILogger logger)
	{
		_logger = logger;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		string requestName = typeof(TRequest).Name;

		_logger.LogInformation("Processing request {RequestName}", requestName);

		TResponse result = await next();

		if (result.IsSuccess)
		{
			_logger.LogInformation("Completed request {RequestName}", requestName);
		}
		else
		{
			using (LogContext.PushProperty("Error", result.Error, true))
			{
				_logger.LogError("Completed request {RequestName} with error", requestName);
			}
		}

		return result;
	}
}