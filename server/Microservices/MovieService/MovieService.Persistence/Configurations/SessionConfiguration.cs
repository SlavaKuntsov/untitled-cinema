using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities;

namespace MovieService.Persistence.Configurations;

public class SessionModelConfiguration : IEntityTypeConfiguration<SessionEntity>
{
	public void Configure(EntityTypeBuilder<SessionEntity> builder)
	{
		builder.ToTable("Session");

		builder.HasKey(s => s.Id);

		builder.Property(s => s.Id)
			.ValueGeneratedOnAdd();

		builder.Property(s => s.StartTime)
			.IsRequired()
			.HasColumnType("timestamptz")
			.HasConversion(
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
			);

		builder.Property(s => s.EndTime)
			.IsRequired()
			.HasColumnType("timestamptz")
			.HasConversion(
				v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
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