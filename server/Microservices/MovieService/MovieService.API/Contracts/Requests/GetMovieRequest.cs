using System.ComponentModel;

namespace MovieService.API.Contracts.Requests;
public class GetMovieRequest
{
	[DefaultValue(10)]
	public byte Limit { get; set; }

	[DefaultValue(1)]
	public byte Offset { get; set; }

	[DefaultValue(null)]
	public string? Filter { get; set; }

	[DefaultValue(null)]
	public string? FilterValue { get; set; }

	[DefaultValue("title")]
	public string SortBy { get; set; }

	[DefaultValue("asc")]
	public string SortDirection { get; set; }
}