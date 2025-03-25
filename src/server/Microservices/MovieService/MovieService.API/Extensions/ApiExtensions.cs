using MediatR;
using MovieService.API.Behaviors;
using MovieService.API.Contracts.RequestExamples.Movies;
using Protobufs.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace MovieService.API.Extensions;

public static class ApiExtensions
{
	public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
	{
		var usersPort = Environment.GetEnvironmentVariable("USERS_APP_PORT");

		if (string.IsNullOrEmpty(usersPort))
			usersPort = configuration.GetValue<string>("ApplicationSettings:UsersPort");

		services.AddGrpcClient<AuthService.AuthServiceClient>(
			options =>
			{
				options.Address = new Uri($"https://localhost:{usersPort}");
			});

		services.AddSwaggerExamplesFromAssemblyOf<CreateMovieRequestExample>();

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		return services;
	}
}