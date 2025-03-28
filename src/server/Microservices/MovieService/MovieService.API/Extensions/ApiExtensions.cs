using FluentValidation;
using MediatR;
using MovieService.API.Behaviors;
using MovieService.API.Contracts.RequestExamples.Movies;
using MovieService.Application.Handlers.Commands.Movies.CreateMovie;
using MovieService.Application.Handlers.Commands.Movies.UpdateMovie;
using Protobufs.Auth;
using Protobufs.Seats;
using Swashbuckle.AspNetCore.Filters;
using Utilities.Validators;

namespace MovieService.API.Extensions;

public static class ApiExtensions
{
	public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
	{
		var usersPort = Environment.GetEnvironmentVariable("USERS_APP_PORT");
		var bookingPort = Environment.GetEnvironmentVariable("BOOKINGS_APP_PORT");

		if (string.IsNullOrEmpty(usersPort))
			usersPort = configuration.GetValue<string>("ApplicationSettings:UsersPort");
		if (string.IsNullOrEmpty(bookingPort))
			bookingPort = configuration.GetValue<string>("ApplicationSettings:BookingsPort");

		services.AddGrpcClient<AuthService.AuthServiceClient>(
			options =>
			{
				options.Address = new Uri($"https://localhost:{usersPort}");
			});
		services.AddGrpcClient<SeatsService.SeatsServiceClient>(
			options =>
			{
				options.Address = new Uri($"https://localhost:{bookingPort}");
			});

		services.AddSwaggerExamplesFromAssemblyOf<CreateMovieRequestExample>();

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

		services.AddValidatorsFromAssemblyContaining<BaseCommandValidator<CreateMovieCommandValidator>>();
		services.AddValidatorsFromAssemblyContaining<BaseCommandValidator<UpdateMovieCommandValidator>>();

		services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

		return services;
	}
}