using System.Globalization;

namespace Extensions.Strings;

public static class StringExtensions
{
	public static bool DateFormatTryParse(this string dateString, out DateTime parsedDateTime)
	{
		return DateTime.TryParseExact(
			dateString,
			Domain.Constants.DateTimeConstants.DATE_FORMAT,
			CultureInfo.InvariantCulture,
			DateTimeStyles.None,
			out parsedDateTime);
	}
}