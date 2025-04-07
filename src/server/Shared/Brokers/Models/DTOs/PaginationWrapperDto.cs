namespace Brokers.Models.DTOs;

public record PaginationWrapperDto<T>
{
	public IList<T> Items { get; init; }
	public byte Limit { get; init; }
	public byte Offset { get; init; }
	public int Total { get; init; }
	public string NextRef { get; init; } = string.Empty;
	public string PrevRef { get; init; } = string.Empty;

	public PaginationWrapperDto(IList<T> items, byte limit, byte offset, int total)
	{
		Items = items;
		Limit = limit;
		Offset = offset;
		Total = total;
	}
}