using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities;

namespace MovieService.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
{
	public void Configure(EntityTypeBuilder<SessionEntity> builder)
	{
		builder.ToTable("Session");

		builder.HasKey(s => s.Id);

		builder.Property(st => st.PriceModifier)
			.HasColumnType("decimal(5, 2)")
			.IsRequired();

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

		builder.HasOne(s => s.Hall)
			.WithMany(h => h.Sessions)
			.HasForeignKey(s => s.HallId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(s => s.Day)
			.WithMany(h => h.Sessions)
			.HasForeignKey(s => s.DayId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}