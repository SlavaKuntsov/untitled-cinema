using System.Reflection;

using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;

namespace MovieService.Persistence;

public class MovieServiceDBContext : DbContext
{
	public DbSet<MovieEntity> Movies { get; set; }
	public DbSet<SessionEntity> Sessions { get; set; }
	public DbSet<CinemaHallEntity> CinemaHalls { get; set; }
	public DbSet<HallSeatEntity> HallSeats { get; set; }

	public MovieServiceDBContext(DbContextOptions<MovieServiceDBContext> options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(modelBuilder);
	}
}