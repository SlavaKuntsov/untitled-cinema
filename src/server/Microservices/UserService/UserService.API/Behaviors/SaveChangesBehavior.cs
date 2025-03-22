using MediatR;

using UserService.Persistence;

namespace UserService.API.Behaviors;

public class SaveChangesBehavior<TRequest, TResponse>(UserServiceDBContext context)
	: IPipelineBehavior<TRequest, TResponse>
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var response = await next();
		await context.SaveChangesAsync(cancellationToken);
		return response;
	}
}