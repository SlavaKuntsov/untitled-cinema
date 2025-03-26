using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookingService.Infrastructure.DateTime;

public class DateTimeConverter : JsonConverter<System.DateTime>
{
	private readonly string _format;

	public DateTimeConverter(string format)
	{
		_format = format;
	}

	public override System.DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return System.DateTime.ParseExact(reader.GetString(), _format, null);
	}

	public override void Write(Utf8JsonWriter writer, System.DateTime value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString(_format));
	}
}