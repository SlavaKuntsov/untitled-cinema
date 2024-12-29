using System.Globalization;

namespace UserService.Application.Extensions;

public static class StringExtensions
{
	public static bool CustomTryParse(this string dateString, out DateTime parsedDateTime)
	{
		return DateTime.TryParseExact(
			dateString,
			Domain.Constants.DateTimeConstants.DATE_TIME_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out parsedDateTime);
	}
}