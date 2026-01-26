using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utilities.DateTime;

public class DateTimeConverter(string format) : JsonConverter<System.DateTime>
{
	public override System.DateTime Read(
		ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options)
	{
		return System.DateTime.ParseExact(reader.GetString(), format, null);
	}

	public override void Write(
		Utf8JsonWriter writer,
		System.DateTime value,
		JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString(format));
	}
}