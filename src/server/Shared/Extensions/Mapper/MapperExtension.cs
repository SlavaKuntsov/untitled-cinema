using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Mapper;

public static class MapperExtension
{
	public static IServiceCollection AddMapper(this IServiceCollection services)
	{
		var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
		typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

		MapsterMapper.Mapper mapperConfig = new(typeAdapterConfig);
		services.AddSingleton<IMapper>(mapperConfig);

		return services;
	}
}