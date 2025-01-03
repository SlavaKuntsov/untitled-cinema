using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Models;

namespace MovieService.Persistence.Configurations;

public class CinemaHallModelConfiguration : IEntityTypeConfiguration<CinemaHallModel>
{
	public void Configure(EntityTypeBuilder<CinemaHallModel> builder)
	{
		builder.ToTable("CinemaHall");

		builder.HasKey(h => h.Id);

		builder.Property(h => h.Id)
			.ValueGeneratedOnAdd();

		builder.Property(h => h.Name)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(h => h.TotalSeats)
			.IsRequired();

		builder.HasMany(h => h.Seats)
			.WithOne(s => s.CinemaHall)
			.HasForeignKey(s => s.HallId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(h => h.Sessions)
			.WithOne(s => s.CinemaHall)
			.HasForeignKey(s => s.HallId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}