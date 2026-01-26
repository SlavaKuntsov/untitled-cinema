using FluentValidation;
using MediatR;

namespace Extensions.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : notnull
{
	public async Task<TResponse> Handle(
		TRequest request,
		RequestHandlerDelegate<TResponse> next,
		CancellationToken cancellationToken)
	{
		var context = new ValidationContext<TRequest>(request);

		var failures = validators
			.Select(v => v.Validate(context))
			.SelectMany(result => result.Errors)
			.Where(f => f != null)
			.ToList();

		if (failures.Count != 0)
			throw new ValidationException(failures);

		return await next();
	}
}