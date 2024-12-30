using MediatR;

using UserService.Persistence;

namespace UserService.API.Behaviors;

public class SaveChangesBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
	private readonly UserServiceDBContext _context;

	public SaveChangesBehavior(UserServiceDBContext context)
	{
		_context = context;
	}

	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var response = await next();
		await _context.SaveChangesAsync(cancellationToken);
		return response;
	}
}