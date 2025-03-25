using System.Globalization;
using Domain.Constants;

namespace Extensions.Strings;

public static class StringExtensions
{
	public static bool DateFormatTryParse(this string dateString, out DateTime parsedDateTime)
	{
		return DateTime.TryParseExact(
			dateString,
			DateTimeConstants.DATE_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out parsedDateTime);
	}

	public static bool DateTimeFormatTryParse(this string dateString, out DateTime parsedDateTime)
	{
		return DateTime.TryParseExact(
			dateString,
			DateTimeConstants.DATE_TIME_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out parsedDateTime);
	}
}