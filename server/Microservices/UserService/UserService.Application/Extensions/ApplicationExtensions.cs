using Microsoft.Extensions.DependencyInjection;

using UserService.Application.Handlers.Commands.Tokens;
using UserService.Application.Handlers.Commands.Users;
using UserService.Application.Handlers.Queries.Users;

namespace UserService.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblyContaining<UserRegistrationCommand.UserRegistrationCommandHandler>();
			cfg.RegisterServicesFromAssemblyContaining<GenerateAndUpdateTokensCommand.GenerateAndUpdateTokensCommandHandler>();
			cfg.RegisterServicesFromAssemblyContaining<LoginQuery.LoginQUeryHandler>();
			cfg.RegisterServicesFromAssemblyContaining<GetUserQuery.GetUserQueryHandler>();
		});

		return services;
	}
}