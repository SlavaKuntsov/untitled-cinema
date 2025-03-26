using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Extensions.Exceptions.Middlewares;

public class GlobalGrpcExceptionInterceptor : Interceptor
{
	public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
		TRequest request,
		ServerCallContext context,
		UnaryServerMethod<TRequest, TResponse> continuation)
	{
		try
		{
			return await base.UnaryServerHandler(request, context, continuation);
		}
		catch (Exception ex)
		{
			throw new RpcException(new Status(StatusCode.Internal, ex.Message));
		}
	}
}