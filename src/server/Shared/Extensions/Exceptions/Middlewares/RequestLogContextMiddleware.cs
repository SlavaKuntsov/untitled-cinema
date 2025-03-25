using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Extensions.Exceptions.Middlewares;

public class RequestLogContextMiddleware(RequestDelegate next)
{
	public Task InvokeAsync(HttpContext httpContext)
	{
		using (LogContext.PushProperty("CorrelationId", httpContext.TraceIdentifier))
		{
			return next(httpContext);
		}
	}
}