using System.Reflection;

using Microsoft.EntityFrameworkCore;

using UserService.Domain.Entities;

namespace UserService.Persistence;

public class UserServiceDBContext : DbContext
{
	public DbSet<UserEntity> Users { get; set; }
	public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
	public DbSet<NotificationEntity> Notifications { get; set; }

	public UserServiceDBContext(DbContextOptions<UserServiceDBContext> options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}