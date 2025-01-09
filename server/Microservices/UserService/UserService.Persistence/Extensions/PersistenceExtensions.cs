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
		services.AddDbContext<UserServiceDBContext>(options =>
		{
			options.UseNpgsql(configuration.GetConnectionString(nameof(UserServiceDBContext)));
		});

		services.AddScoped<IUsersRepository, UsersRepository>();
		services.AddScoped<ITokensRepository, TokensRepository>();

		return services;
	}
}