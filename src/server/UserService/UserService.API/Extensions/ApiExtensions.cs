using System.Reflection;
using Extensions.Exceptions.Middlewares;
using Mapster;
using MapsterMapper;
using MediatR;
using Swashbuckle.AspNetCore.Filters;
using UserService.API.Behaviors;
using UserService.API.Contracts.Examples;
using UserService.API.Controllers.Grpc;
using UserService.Infrastructure.Hubs.Notification;
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
		
		var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
		typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());

		Mapper mapperConfig = new(typeAdapterConfig);
		services.AddSingleton<IMapper>(mapperConfig);

		return services;
	}

	public static WebApplication AddAPI(this WebApplication app)
	{
		app.MapGrpcService<AuthController>();

		app.MapHub<NotificationHub>("/notificationsHub");

		return app;
	}
}