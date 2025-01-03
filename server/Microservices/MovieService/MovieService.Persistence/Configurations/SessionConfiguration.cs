using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Models;

namespace MovieService.Persistence.Configurations;

public class SessionModelConfiguration : IEntityTypeConfiguration<SessionModel>
{
	public void Configure(EntityTypeBuilder<SessionModel> builder)
	{
		builder.ToTable("Session");

		builder.HasKey(s => s.Id);

		builder.Property(s => s.Id)
			.ValueGeneratedOnAdd();

		builder.Property(s => s.StartTime)
			.IsRequired()
			.HasColumnType("timestamp")
			.HasConversion(
				v => v,
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.Property(s => s.EndTime)
			.IsRequired()
			.HasColumnType("timestamp")
			.HasConversion(
				v => v,
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.HasOne(s => s.Movie)
			.WithMany(m => m.Sessions)
			.HasForeignKey(s => s.MovieId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(s => s.CinemaHall)
			.WithMany(h => h.Sessions)
			.HasForeignKey(s => s.HallId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}