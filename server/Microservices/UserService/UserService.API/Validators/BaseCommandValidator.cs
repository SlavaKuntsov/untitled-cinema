using FluentValidation;

namespace UserService.API.Validators;

public abstract class BaseCommandValidator<T> : AbstractValidator<T>
{
	protected BaseCommandValidator() { }
}