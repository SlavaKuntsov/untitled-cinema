using System.Reflection;
using Extensions.Exceptions.Middlewares;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Filters;
using UserService.API.Behaviors;
using UserService.API.Contracts.Examples;
using UserService.Infrastructure.Hubs.Notification;

namespace UserService.API.Extensions;

public static class ApiExtensions
{
	public static IServiceCollection AddAPI(this IServiceCollection services)
	{
		// services.AddGrpc(
		// 	options =>
		// 	{
		// 		options.Interceptors.Add<GlobalGrpcExceptionInterceptor>();
		// 	});

		services.AddSwaggerExamplesFromAssemblyOf<CreateUserRequestExample>();

		var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
		var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");

		if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
			throw new Exception("Google client data is null.");

		services
			.AddAuthentication(
				options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
			.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddGoogle(
				GoogleDefaults.AuthenticationScheme,
				options =>
				{
					options.ClientId = clientId;
					options.ClientSecret = clientSecret;
					options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.CallbackPath = "/google-response";
				});

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
		// app.MapGrpcService<AuthController>();

		app.MapHub<NotificationHub>("/notificationsHub");

		return app;
	}
}