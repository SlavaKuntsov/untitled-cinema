using System.ComponentModel;
using System.Reflection;

namespace Extensions.Enums;

public static class EnumExtensions
{
	public static string GetDescription(this Enum value)
	{
		FieldInfo field = value.GetType().GetField(value.ToString());
		DescriptionAttribute attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));
		return attribute == null ? value.ToString() : attribute.Description;
	}
}