using System.Globalization;

using FluentValidation;

using UserService.Application.Handlers.Commands.Users;

namespace UserService.API.Validators.Users;

public class UserRegistrationCommandValidator<T> : BaseCommandValidator<UserRegistrationCommand>
{
	public UserRegistrationCommandValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email cannot be null or empty.")
			.EmailAddress().WithMessage("Invalid email format.");

		RuleFor(m => m.Password)
			.NotEmpty().WithMessage("Password cannot be null or empty.")
			.MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
			.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
			.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
			.Matches("[0-9]").WithMessage("Password must contain at least one number.")
			.Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

		RuleFor(user => user.Role)
			.NotNull()
			.IsInEnum();

		// FirstName, LastName, и DateOfBirth могут быть null, но если они не null — валидируем
		When(x => x.FirstName != null, () =>
		{
			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("FirstName cannot be empty if provided.");
		});

		When(x => x.LastName != null, () =>
		{
			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("LastName cannot be empty if provided.");
		});

		When(x => x.DateOfBirth != null, () =>
		{
			RuleFor(x => x.DateOfBirth)
				.Must(BeAValidDate).WithMessage("Date of birth must be in a valid format.");
		});
	}

	private bool BeAValidDate(string? dateOfBirth)
	{
		if (string.IsNullOrWhiteSpace(dateOfBirth))
			return false;

		return DateTime.TryParseExact(
			dateOfBirth,
			Constants.DateTimeConstants.DATE_TIME_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out _);
	}
}