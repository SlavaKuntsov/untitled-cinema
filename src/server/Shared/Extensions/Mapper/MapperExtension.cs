using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Mapper;

public static class MapperExtension
{
	public static IServiceCollection AddMapper(this IServiceCollection services)
	{

		return services;
	}
}