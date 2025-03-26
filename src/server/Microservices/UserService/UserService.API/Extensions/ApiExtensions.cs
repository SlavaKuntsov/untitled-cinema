using Extensions.Exceptions.Middlewares;
using MediatR;
using Swashbuckle.AspNetCore.Filters;
using UserService.API.Behaviors;
using UserService.API.Contracts.Examples;
using UserService.API.Controllers.Grpc;
using UserService.Infrastructure.Notification;

namespace UserService.API.Extensions;

public static class ApiExtensions
{
	public static IServiceCollection AddAPI(this IServiceCollection services)
	{
		services.AddGrpc(
			options =>
			{
				options.Interceptors.Add<GlobalGrpcExceptionInterceptor>();
			});

		services.AddSwaggerExamplesFromAssemblyOf<CreateUserRequestExample>();

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		return services;
	}

	public static WebApplication AddAPI(this WebApplication app)
	{
		app.MapGrpcService<AuthController>();

		app.MapHub<NotificationHub>("/notify");

		return app;
	}
}