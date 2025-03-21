using System.Globalization;

namespace MovieService.Application.Validators;

public static class GeneralValidator
{
	public static bool BeAValidDate(string? dateOfBirth)
	{
		if (string.IsNullOrWhiteSpace(dateOfBirth))
			return false;

		return DateTime.TryParseExact(
			dateOfBirth,
			Domain.Constants.DateTimeConstants.DATE_TIME_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out _);
	}
}
