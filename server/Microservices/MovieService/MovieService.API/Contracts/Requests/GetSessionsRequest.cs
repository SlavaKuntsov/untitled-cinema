using System.ComponentModel;

namespace MovieService.API.Contracts.Requests;

public class GetSessionsRequest
{
	[DefaultValue(10)]
	public byte Limit { get; set; } = 10;

	[DefaultValue(1)]
	public byte Offset { get; set; } = 1;

	[DefaultValue(null)]
	public string? Date { get; set; }

	[DefaultValue(null)]
	public string? Hall { get; set; }
}