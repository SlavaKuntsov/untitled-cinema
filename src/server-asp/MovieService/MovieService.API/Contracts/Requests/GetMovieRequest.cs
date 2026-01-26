using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace MovieService.API.Contracts.Requests;

public class GetMovieRequest
{
	[FromQuery]
	[DefaultValue(10)]
	public byte Limit { get; set; }

	[FromQuery]
	[DefaultValue(1)]
	public byte Offset { get; set; }

	[FromQuery(Name = "Filter")]
	public string[] Filters { get; set; } = [];

	[FromQuery(Name = "FilterValue")]
	public string[] FilterValues { get; set; } = [];

	[FromQuery]
	[DefaultValue("title")]
	public string SortBy { get; set; }

	[FromQuery]
	[DefaultValue("asc")]
	public string SortDirection { get; set; }

	[FromQuery]
	[DefaultValue(null)]
	public string? Date { get; set; } = string.Empty;
}