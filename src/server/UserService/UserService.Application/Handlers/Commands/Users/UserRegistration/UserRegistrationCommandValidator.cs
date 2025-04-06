using System.Globalization;
using Domain.Constants;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Utilities.Validators;

namespace UserService.Application.Handlers.Commands.Users.UserRegistration;

public class UserRegistrationCommandValidator : BaseCommandValidator<UserRegistrationCommand>
{
	public UserRegistrationCommandValidator(ILogger<UserRegistrationCommandValidator> logger)
	{
		logger.LogError("UserRegistrationCommandValidator initialized");
		
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email cannot be null or empty.")
			.EmailAddress().WithMessage("Invalid email format.");

		RuleFor(m => m.Password)
			.NotEmpty().WithMessage("Password cannot be null or empty.")
			.MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
			.Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
			.Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
			.Matches("[0-9]").WithMessage("Password must contain at least one number.");

		RuleFor(x => x.FirstName)
			.NotEmpty().WithMessage("FirstName cannot be null or empty.");

		RuleFor(x => x.LastName)
			.NotEmpty().WithMessage("LastName cannot be null or empty.");

		RuleFor(x => x.DateOfBirth)
			.NotEmpty().WithMessage("DateOfBirth cannot be null or empty.")
			.Must(BeAValidDate).WithMessage("Date of birth must be in a valid format.");
	}

	private bool BeAValidDate(string? dateOfBirth)
	{
		if (string.IsNullOrWhiteSpace(dateOfBirth))
			return false;

		return DateTime.TryParseExact(
			dateOfBirth,
			DateTimeConstants.DATE_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out _);
	}
}