using System.Reflection;

using Microsoft.EntityFrameworkCore;

using MovieService.Domain;
using MovieService.Domain.Models;

namespace MovieService.Persistence;

public class MovieServiceDBContext : DbContext
{
	public DbSet<MovieModel> Movies { get; set; }
	public DbSet<SessionModel> Sessions { get; set; }
	public DbSet<CinemaHallModel> CinemaHalls { get; set; }
	public DbSet<HallSeatModel> HallSeats { get; set; }

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