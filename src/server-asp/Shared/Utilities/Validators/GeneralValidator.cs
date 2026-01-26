using System.Globalization;
using Domain.Constants;

namespace Utilities.Validators;

public static class GeneralValidator
{
	public static bool BeAValidDate(string? dateOfBirth)
	{
		if (string.IsNullOrWhiteSpace(dateOfBirth))
			return false;

		return System.DateTime.TryParseExact(
			dateOfBirth,
			DateTimeConstants.DATE_TIME_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out _);
	}
}