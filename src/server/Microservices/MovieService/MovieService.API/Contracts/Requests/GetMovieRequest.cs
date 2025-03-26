using System.ComponentModel;

using Microsoft.AspNetCore.Mvc;

namespace MovieService.API.Contracts.Requests;

public class GetMovieRequest
{
	[DefaultValue(10)]
	public byte Limit { get; set; }
	[DefaultValue(1)]
	public byte Offset { get; set; }
	[FromQuery(Name = "Filter")]
	public string[] Filters { get; set; } = [];
	[FromQuery(Name = "FilterValue")]
	public string[] FilterValues { get; set; } = [];
	[DefaultValue("title")]
	public string SortBy { get; set; }
	[DefaultValue("asc")]
	public string SortDirection { get; set; }
	[DefaultValue(null)]
	public string? Date { get; set; } = string.Empty;
}