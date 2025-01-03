using Microsoft.Extensions.DependencyInjection;
using MovieService.Application.Handlers.Commands.Users.UserRegistration;

namespace MovieService.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblyContaining<UserRegistrationCommandHandler>();
		});

		return services;
	}
}