using Microsoft.Extensions.DependencyInjection;
using MovieService.Application.Consumers;
using MovieService.Application.Handlers.Commands.Movies.CreateMovie;

namespace MovieService.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddHostedService<BookingPriceConsumeService>();
		services.AddHostedService<SessionSeatsConsumerServices>();

		services.AddMediatR(
			cfg =>
			{
				cfg.RegisterServicesFromAssemblyContaining<CreateMovieCommandHandler>();
			});
		//
		// services.AddValidatorsFromAssemblyContaining<BaseCommandValidator<CreateMovieCommandValidator>>();
		// services.AddValidatorsFromAssemblyContaining<BaseCommandValidator<UpdateMovieCommandValidator>>();

		return services;
	}
}