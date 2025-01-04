using Microsoft.Extensions.DependencyInjection;

using MovieService.Application.Handlers.Commands.Movies.Create;

namespace MovieService.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblyContaining<CreateMovieCommandHandler>();
		});

		return services;
	}
}