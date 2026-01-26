using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MovieService.Domain.Entities;

namespace MovieService.Persistence.Configurations;

public class SeatTypeConfiguration : IEntityTypeConfiguration<SeatTypeEntity>
{
	public void Configure(EntityTypeBuilder<SeatTypeEntity> builder)
	{
		builder.ToTable("SeatType");

		builder.HasKey(st => st.Id);

		builder.Property(st => st.Name)
			.IsRequired()
			.HasMaxLength(50);

		builder.Property(st => st.PriceModifier)
			.HasColumnType("decimal(5, 2)")
			.IsRequired();

		builder.HasMany(st => st.Seats)
			.WithOne(s => s.SeatType)
			.HasForeignKey(s => s.SeatTypeId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasData(
			new SeatTypeEntity
			{
				Id = Guid.NewGuid(),
				Name = "Стандарт",
				PriceModifier = 1
			});
	}
}