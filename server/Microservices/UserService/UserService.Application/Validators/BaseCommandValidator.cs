using System.Globalization;

using FluentValidation;

namespace MovieService.Application.Validators;

public abstract class BaseCommandValidator<T> : AbstractValidator<T>
{
	protected BaseCommandValidator() 
	{
	}
}