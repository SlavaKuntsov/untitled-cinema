using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities;

namespace MovieService.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<SeatEntity>
{
	public void Configure(EntityTypeBuilder<SeatEntity> builder)
	{
		builder.ToTable("Seat");

		builder.HasKey(s => s.Id);

		builder.Property(s => s.Row)
			.IsRequired();

		builder.Property(s => s.Column)
			.IsRequired();

		builder.Property(s => s.Exists)
			.HasDefaultValue(true);

		builder.HasOne(s => s.SeatType)
			.WithMany(st => st.Seats)
			.HasForeignKey(s => s.SeatTypeId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}