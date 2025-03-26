using System.ComponentModel;
using System.Reflection;
using BookingService.Domain.Enums;
using Extensions.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace BookingService.Infrastructure.Serializers;

public class BookingStatusSerialization : IBsonSerializer<BookingStatus>
{
	public Type ValueType => typeof(BookingStatus);

	public BookingStatus Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
	{
		var description = context.Reader.ReadString();

		foreach (var field in typeof(BookingStatus).GetFields(BindingFlags.Public | BindingFlags.Static))
		{
			var attribute = field.GetCustomAttribute<DescriptionAttribute>();

			if (attribute != null && attribute.Description == description)
				return (BookingStatus)field.GetValue(null);
		}

		throw new BsonSerializationException($"Cannot convert '{description}' to {nameof(BookingStatus)}.");
	}

	public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, BookingStatus value)
	{
		var description = value.GetDescription();
		context.Writer.WriteString(description);
	}

	object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
	{
		return Deserialize(context, args);
	}

	void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
	{
		if (value is BookingStatus bookingStatus)
			Serialize(context, args, bookingStatus);
		else
			throw new BsonSerializationException(
				$"Expected value of type {nameof(BookingStatus)}, but got {value.GetType()}.");
	}
}