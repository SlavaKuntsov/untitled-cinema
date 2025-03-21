using FluentValidation;

using MediatR;

namespace BookingService.API.Behaviors
{
	public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : notnull
	{
		private readonly IEnumerable<IValidator<TRequest>> _validators;

		public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
		{
			_validators = validators;
		}

		public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
		{
			if (_validators.Any())
			{
				var context = new ValidationContext<TRequest>(request);

				var validationResults = await Task.WhenAll(
					_validators.Select(v => v.ValidateAsync(context, cancellationToken))
				);

				var failures = validationResults
					.SelectMany(result => result.Errors)
					.Where(f => f != null)
					.ToList();

				if (failures.Any())
				{
					var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
					throw new ValidationException($"Validation failed: {string.Join("; ", errorMessages)}");
				}
			}

			return await next();
		}
	}
}
