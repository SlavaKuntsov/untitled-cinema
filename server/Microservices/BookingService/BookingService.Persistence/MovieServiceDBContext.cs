using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace BookingService.Persistence;

public class BookingServiceDBContext : DbContext
{
	//public DbSet<MovieEntity> Movies { get; set; }

	public BookingServiceDBContext(DbContextOptions<BookingServiceDBContext> options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}