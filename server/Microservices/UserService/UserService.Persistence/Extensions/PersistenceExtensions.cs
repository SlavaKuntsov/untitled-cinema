using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using UserService.Domain.Interfaces.Repositories;
using UserService.Persistence.Repositories;

namespace UserService.Persistence.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

		if (string.IsNullOrEmpty(connectionString))
			connectionString = configuration.GetConnectionString("UserServiceDBContext");

		services.AddDbContext<UserServiceDBContext>(options =>
		{
			options.UseNpgsql(connectionString);
		});

		services.AddScoped<IUsersRepository, UsersRepository>();
		services.AddScoped<ITokensRepository, TokensRepository>();

		return services;
	}
}