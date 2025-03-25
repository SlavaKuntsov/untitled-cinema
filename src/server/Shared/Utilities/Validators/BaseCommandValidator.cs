using FluentValidation;

namespace Utilities.Validators;

public abstract class BaseCommandValidator<T> : AbstractValidator<T>;