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

		services.AddDbContextPool<UserServiceDBContext>(options =>
		{
			options.UseNpgsql(connectionString);
		}, poolSize: 128);

		services.AddScoped<IUsersRepository, UsersRepository>();
		services.AddScoped<ITokensRepository, TokensRepository>();
		services.AddScoped<INotificationsRepository, NotificationsRepository>();

		return services;
	}
}