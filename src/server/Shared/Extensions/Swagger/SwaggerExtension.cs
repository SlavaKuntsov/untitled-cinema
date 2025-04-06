using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Extensions.Swagger;

public static class SwaggerExtension
{
	public static IServiceCollection AddSwagger(this IServiceCollection services)
	{
		services.AddSwaggerGen(
			options =>
			{
				options.ExampleFilters();

				options.AddSecurityDefinition(
					"Bearer",
					new OpenApiSecurityScheme
					{
						Name = "Authorization",
						In = ParameterLocation.Header,
						Type = SecuritySchemeType.Http,
						Scheme = "Bearer",
						BearerFormat = "JWT",
						Description = "Введите токен JWT в формате 'Bearer {токен}'"
					});

				options.AddSecurityRequirement(
					new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
							{
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
								}
							},
							Array.Empty<string>()
						}
					});
			});

		return services;
	}
}