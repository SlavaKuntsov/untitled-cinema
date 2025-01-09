using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Handlers.Commands.Users.UserRegistration;

namespace UserService.Application.Extensions;

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