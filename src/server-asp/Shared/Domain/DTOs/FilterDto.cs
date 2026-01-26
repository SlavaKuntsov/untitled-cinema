namespace Domain.DTOs;

public struct FilterDto
{
	public string Field { get; set; }
	public string Value { get; set; }

	public FilterDto(string field, string value)
	{
		Field = field;
		Value = value;
	}
}