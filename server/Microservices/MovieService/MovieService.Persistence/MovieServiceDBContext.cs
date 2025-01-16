using System.Reflection;

using Microsoft.EntityFrameworkCore;

using MovieService.Domain.Entities;

namespace MovieService.Persistence;

public class MovieServiceDBContext : DbContext
{
	public DbSet<MovieEntity> Movies { get; set; }
	public DbSet<GenreEntity> Genres { get; set; }
	public DbSet<MovieGenreEntity> MovieGenres { get; set; }
	public DbSet<SessionEntity> Sessions { get; set; }
	public DbSet<HallEntity> Halls { get; set; }
	public DbSet<SeatEntity> Seats { get; set; }
	public DbSet<SeatTypeEntity> SeatTypes { get; set; }
	public DbSet<DayEntity> Days { get; set; }

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