using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities;

namespace MovieService.Persistence.Configurations;

public class HallSeatConfiguration : IEntityTypeConfiguration<HallSeatEntity>
{
	public void Configure(EntityTypeBuilder<HallSeatEntity> builder)
	{
		builder.ToTable("HallSeat");

		builder.HasKey(s => s.Id);

		builder.Property(s => s.SeatRow)
			.IsRequired();

		builder.Property(s => s.SeatColumn)
			.IsRequired();

		builder.Property(s => s.Exists)
			.HasDefaultValue(true);

		builder.Property(s => s.SeatType)
			.HasMaxLength(50);

		builder.Property(s => s.Price)
			.HasColumnType("decimal(10, 2)")
			.HasDefaultValue(0.00m);
	}
}