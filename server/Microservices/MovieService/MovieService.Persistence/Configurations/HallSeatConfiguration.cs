using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Models;

namespace MovieService.Persistence.Configurations;

public class HallSeatModelConfiguration : IEntityTypeConfiguration<HallSeatModel>
{
	public void Configure(EntityTypeBuilder<HallSeatModel> builder)
	{
		builder.ToTable("HallSeat");

		builder.HasKey(s => s.Id);

		builder.Property(s => s.Id)
			.ValueGeneratedOnAdd();

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