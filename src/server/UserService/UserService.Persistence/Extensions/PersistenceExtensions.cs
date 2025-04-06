using Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Data;
using UserService.Domain.Interfaces.Repositories;
using UserService.Persistence.Repositories;

namespace UserService.Persistence.Extensions;

public static class PersistenceExtensions
{
	public static IServiceCollection AddPersistence(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddPostgres<IDBContext, UserServiceDBContext>(configuration);

		services.AddScoped<IUsersRepository, UsersRepository>();
		services.AddScoped<ITokensRepository, TokensRepository>();
		services.AddScoped<INotificationsRepository, NotificationsRepository>();

		return services;
	}
}