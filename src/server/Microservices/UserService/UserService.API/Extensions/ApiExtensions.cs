using Extensions.Exceptions.Middlewares;
using FluentValidation;
using MediatR;
using MovieService.Application.Validators;
using Swashbuckle.AspNetCore.Filters;
using UserService.API.Behaviors;
using UserService.API.Consumers;
using UserService.API.Contracts.Examples;
using UserService.API.Controllers.Grpc;
using UserService.Application.Handlers.Commands.Users.UserRegistration;
using UserService.Infrastructure.Notification;

namespace UserService.API.Extensions;

public static class ApiExtensions
{
	public static IServiceCollection AddAPI(this IServiceCollection services)
	{
		services.AddHostedService<BookingPayConsumeService>();

		services.AddGrpc(options => { options.Interceptors.Add<GlobalGrpcExceptionInterceptor>(); });
		services.AddSwaggerExamplesFromAssemblyOf<CreateUserRequestExample>();

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

		services.AddValidatorsFromAssemblyContaining<BaseCommandValidator<UserRegistrationCommand>>();

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(global::Extensions.Behaviors.ValidationBehavior<,>));
		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SaveChangesBehavior<,>));

		return services;
	}
	
	public static WebApplication AddAPI(this WebApplication app)
	{
		app.MapGrpcService<AuthController>();

		app.MapHub<NotificationHub>("/notify");

		return app;
	}
}