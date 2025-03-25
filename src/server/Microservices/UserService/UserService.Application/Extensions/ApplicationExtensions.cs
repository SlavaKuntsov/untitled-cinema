using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Consumers;
using UserService.Application.Handlers.Commands.Users.UserRegistration;
using Utilities.Validators;

namespace UserService.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddHostedService<BookingPayConsumeService>();

		services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblyContaining<UserRegistrationCommandHandler>(); });

		// services.AddValidatorsFromAssemblyContaining<BaseCommandValidator<UserRegistrationCommand>>();
		services.AddValidatorsFromAssemblyContaining<BaseCommandValidator<UserRegistrationCommandValidator>>();

		return services;
	}
}