using FluentValidation;

namespace MovieService.API.Validators;

public abstract class BaseCommandValidator<T> : AbstractValidator<T>
{
	protected BaseCommandValidator() { }
}