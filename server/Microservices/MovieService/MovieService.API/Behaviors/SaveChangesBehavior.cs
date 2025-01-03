using MediatR;

using MovieService.Persistence;

namespace MovieService.API.Behaviors;

public class SaveChangesBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
	private readonly MovieServiceDBContext _context;

	public SaveChangesBehavior(MovieServiceDBContext context)
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