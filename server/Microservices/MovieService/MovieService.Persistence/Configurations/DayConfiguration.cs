using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities;

namespace MovieService.Persistence.Configurations;

public class DayConfiguration : IEntityTypeConfiguration<DayEntity>
{
	public void Configure(EntityTypeBuilder<DayEntity> builder)
	{
		builder.ToTable("Day");

		builder.HasKey(h => h.Id);

		//builder.Property(p => p.Date)
		//	.IsRequired()
		//	.HasColumnType("date")
		//	.HasConversion(
		//		v => v.Date,
		//		v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
		//	);

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

		builder.HasMany(h => h.Sessions)
			.WithOne(s => s.Day)
			.HasForeignKey(s => s.DayId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}